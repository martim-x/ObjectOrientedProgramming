using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Project.Data
{
    // ── PostgreDbContext ────────────────────────────────────────────────────────

    public class PostgreDbContext : DbContext
    {
        public DbSet<App> Apps { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserApp> UserApps { get; set; } = null!; // M2M

        public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
            : base(options) { }

        // Конструктор без параметров — нужен только для EF-инструментов
        public PostgreDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(PostgreSQLRep.LoadConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>(e =>
            {
                e.HasKey(a => a.Id);

                // List<string> ↔ строка через запятую в БД
                e.Property(a => a.Tags)
                    .HasConversion(
                        v => string.Join(",", v),
                        v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
                    );

                // Вычисляемые поля — не хранятся в БД
                e.Ignore(a => a.FinalPrice);
                e.Ignore(a => a.ButtonLabel);
                // IsDownloaded вычисляется из UserApps — в колонку не пишем
                e.Ignore(a => a.IsDownloaded);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.Ignore(u => u.AvatarLetter);
            });

            // M2M: составной PK (UserId + AppId) — одна запись на пару юзер+приложение
            modelBuilder.Entity<UserApp>(e =>
            {
                e.HasKey(ua => new { ua.UserId, ua.AppId });
            });
        }
    }

    // ── PostgreSQLRep ───────────────────────────────────────────────────────────

    /// <summary>
    /// Репозиторий на базе PostgreSQL (EF Core + Npgsql).
    /// Строка подключения читается из .env-файла рядом с exe.
    /// currentUserId — ID залогиненного юзера для работы с M2M UserApps.
    /// </summary>
    public class PostgreSQLRep : BaseRepository
    {
        private readonly PostgreDbContext _db;
        private readonly Guid _userId; // Текущий пользователь сессии

        public PostgreSQLRep(Guid currentUserId = default)
        {
            _userId = currentUserId;

            var options = new DbContextOptionsBuilder<PostgreDbContext>()
                .UseNpgsql(LoadConnectionString())
                .Options;

            _db = new PostgreDbContext(options);
            _db.Database.EnsureCreated(); // Создаёт таблицы, если их нет

            if (!_db.Apps.Any())
                SeedAppsToDb();
            if (!_db.Users.Any())
                SeedUsersToDb();
        }

        // ── .env → строка подключения Npgsql ───────────────────────────────────

        public static string LoadConnectionString()
        {
            var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
            if (!File.Exists(envPath))
                envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

            string host = "localhost",
                port = "5433",
                database = "postgres",
                username = "postgre",
                password = "111";

            if (File.Exists(envPath))
            {
                foreach (var raw in File.ReadAllLines(envPath))
                {
                    var line = raw.Trim();
                    if (line.StartsWith('#') || !line.Contains('='))
                        continue;

                    var eqIdx = line.IndexOf('=');
                    var key = line[..eqIdx].Trim();
                    var val = line[(eqIdx + 1)..].Trim().Trim('"');

                    switch (key)
                    {
                        case "DB_HOST":
                            host = val;
                            break;
                        case "DB_PORT_FROM":
                            port = val;
                            break;
                        case "DB_NAME":
                            database = val;
                            break;
                        case "DB_ADMIN":
                            username = val;
                            break;
                        case "DB_ADMIN_PASS":
                            password = val;
                            break;
                    }
                }
            }

            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        // ── Apps ────────────────────────────────────────────────────────────────

        // public override List<App> GetAllApps()
        // {
        //     // Одним запросом берём ID приложений, установленных текущим юзером
        //     var installed = _db
        //         .UserApps.Where(ua => ua.UserId == _userId && ua.IsInstalled)
        //         .Select(ua => ua.AppId)
        //         .ToHashSet();

        //     var apps = _db.Apps.ToList();
        //     foreach (var app in apps)
        //         app.IsDownloaded = installed.Contains(app.Id);

        //     return apps;
        // }

        // public override App? GetAppById(Guid id)
        // {
        //     var app = _db.Apps.FirstOrDefault(a => a.Id == id);
        //     if (app != null)
        //     {
        //         var record = _db.UserApps.Find(_userId, id);
        //         app.IsDownloaded = record?.IsInstalled ?? false;
        //     }
        //     return app;
        // }
        public override List<App> GetAllApps()
        {
            var apps = _db.Apps.ToList();

            var installedIds = _db
                .UsersApps.Where(x => x.UserId == _userId)
                .Select(x => x.AppId)
                .ToHashSet();

            foreach (var app in apps)
                app.IsDownloaded = installedIds.Contains(app.Id);

            return apps;
        }

        public override App? GetAppById(Guid id)
        {
            var app = _db.Apps.Find(id);
            if (app == null)
                return null;

            app.IsDownloaded = _db.UsersApps.Any(x => x.UserId == _userId && x.AppId == id);

            return app;
        }

        public override void AddApp(App app)
        {
            _db.Apps.Add(app);
            _db.SaveChanges();
        }

        public override void UpdateApp(App app)
        {
            _db.Apps.Update(app);
            _db.SaveChanges();
        }

        public override void DeleteApp(Guid id)
        {
            var item = _db.Apps.Find(id);
            if (item == null)
                return;
            _db.Apps.Remove(item);
            _db.SaveChanges();
        }

        public override void DownloadApp(Guid appId)
        {
            var app = _db.Apps.Find(appId);
            if (app == null)
                return;

            app.DownloadCount++;

            var record = _db.UsersApps.Find(_userId, appId);
            if (record == null)
            {
                _db.UsersApps.Add(
                    new UsersApps
                    {
                        UserId = _userId,
                        AppId = appId,
                        InstalledAt = DateTime.UtcNow,
                    }
                );
            }
            else
            {
                record.InstalledAt = DateTime.UtcNow;
            }

            _db.SaveChanges();
        }

        public override void UninstallApp(Guid appId)
        {
            var app = _db.Apps.Find(appId);
            if (app == null)
                return;

            if (app.DownloadCount > 0)
                app.DownloadCount--;

            var record = _db.UsersApps.Find(_userId, appId);
            if (record != null)
                _db.UsersApps.Remove(record);

            _db.SaveChanges();
        }

        public override void RestoreDefaults()
        {
            _db.Apps.RemoveRange(_db.Apps);
            _db.SaveChanges();
            SeedAppsToDb();
        }

        // ── Users ────────────────────────────────────────────────────────────────

        public override List<User> GetAllUsers() => _db.Users.ToList();

        public override User? GetUserByLogin(string login) =>
            _db.Users.FirstOrDefault(u => u.Login.ToLower() == login.ToLower());

        public override void AddUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public override void UpdateUser(User user)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        // ── Вставка Seed в БД ────────────────────────────────────────────────────

        private void SeedAppsToDb()
        {
            foreach (var app in SeedApps()) // SeedApps() из BaseRepository
                _db.Apps.Add(app);
            _db.SaveChanges();
        }

        private void SeedUsersToDb()
        {
            foreach (var user in SeedUsers()) // SeedUsers() из BaseRepository
                _db.Users.Add(user);
            _db.SaveChanges();
        }
    }
}
