using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Project.Data
{
    public class PostgreDbContext : DbContext
    {
        public DbSet<App> Apps { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UsersApps> UserAppLinks { get; set; } = null!;

        public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
            : base(options) { }

        public PostgreDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(PostgreSQLRepEF.LoadConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>(e =>
            {
                e.ToTable("apps");
                e.HasKey(a => a.Id);

                e.Property(a => a.Tags)
                    .HasConversion(
                        v => string.Join(",", v),
                        v =>
                            string.IsNullOrWhiteSpace(v)
                                ? new List<string>()
                                : v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
                    );

                e.Ignore(a => a.FinalPrice);
                e.Ignore(a => a.ButtonLabel);
                e.Ignore(a => a.IsDownloaded);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(u => u.Id);
                e.Ignore(u => u.AvatarLetter);
            });

            modelBuilder.Entity<UsersApps>(e =>
            {
                e.ToTable("users_apps");
                e.HasKey(x => new { x.UserId, x.AppId });

                e.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<App>()
                    .WithMany()
                    .HasForeignKey(x => x.AppId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

    public class PostgreSQLRepEF : BaseRepository
    {
        private readonly PostgreDbContext _db;
        private readonly Guid _userId;

        public PostgreSQLRepEF(Guid currentUserId = default)
        {
            _userId = currentUserId;

            var options = new DbContextOptionsBuilder<PostgreDbContext>()
                .UseNpgsql(LoadConnectionString())
                .Options;

            _db = new PostgreDbContext(options);

            EnsureDatabaseCreated();

            if (!_db.Apps.Any())
                SeedAppsToDb();

            if (!_db.Users.Any())
                SeedUsersToDb();
        }

        public static string LoadConnectionString()
        {
            var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
            if (!File.Exists(envPath))
                envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

            string host = "0.0.0.0",
                port = "5433",
                database = "postgres",
                username = "postgres",
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

        private void EnsureDatabaseCreated()
        {
            _db.Database.ExecuteSqlRaw(
                """
                    create table if not exists apps
                    (
                        id uuid primary key,
                        short_name text not null,
                        full_name text not null,
                        description text not null,
                        developer text not null,
                        category text not null,
                        rating double precision not null,
                        rating_count integer not null,
                        price double precision not null,
                        version text not null,
                        size_mb double precision not null,
                        country text not null,
                        age_rating text not null,
                        color text not null,
                        is_featured boolean not null,
                        is_in_stock boolean not null,
                        download_count integer not null,
                        discount_percent double precision null,
                        release_date timestamp without time zone not null,
                        tags text not null
                    );

                    create table if not exists users
                    (
                        id uuid primary key,
                        login text not null unique,
                        password_hash text not null,
                        first_name text null,
                        last_name text null,
                        email text null,
                        role integer not null,
                        avatar_color text not null
                    );

                    create table if not exists users_apps
                    (
                        user_id uuid not null,
                        app_id uuid not null,
                        installed_at timestamp without time zone not null,
                        primary key (user_id, app_id),
                        constraint fk_users_apps_user
                            foreign key (user_id) references users(id) on delete cascade,
                        constraint fk_users_apps_app
                            foreign key (app_id) references apps(id) on delete cascade
                    );
                """
            );
        }

        public override List<App> GetAllApps()
        {
            var apps = _db.Apps.ToList();

            var installedIds =
                _userId == Guid.Empty
                    ? new HashSet<Guid>()
                    : _db
                        .UserAppLinks.Where(x => x.UserId == _userId)
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

            app.IsDownloaded =
                _userId != Guid.Empty
                && _db.UserAppLinks.Any(x => x.UserId == _userId && x.AppId == id);

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

            var links = _db.UserAppLinks.Where(x => x.AppId == id).ToList();
            if (links.Count > 0)
                _db.UserAppLinks.RemoveRange(links);

            _db.Apps.Remove(item);
            _db.SaveChanges();
        }

        public override void DownloadApp(Guid appId)
        {
            if (_userId == Guid.Empty)
                return;

            var app = _db.Apps.Find(appId);
            if (app == null)
                return;

            var record = _db.UserAppLinks.Find(_userId, appId);

            if (record == null)
            {
                _db.UserAppLinks.Add(
                    new UsersApps
                    {
                        UserId = _userId,
                        AppId = appId,
                        InstalledAt = DateTime.UtcNow,
                    }
                );

                app.DownloadCount++;
            }
            else
            {
                record.InstalledAt = DateTime.UtcNow;
            }

            _db.SaveChanges();
        }

        public override void UninstallApp(Guid appId)
        {
            if (_userId == Guid.Empty)
                return;

            var app = _db.Apps.Find(appId);
            if (app == null)
                return;

            var record = _db.UserAppLinks.Find(_userId, appId);
            if (record == null)
                return;

            _db.UserAppLinks.Remove(record);

            if (app.DownloadCount > 0)
                app.DownloadCount--;

            _db.SaveChanges();
        }

        public override void RestoreDefaults()
        {
            _db.UserAppLinks.RemoveRange(_db.UserAppLinks);
            _db.Apps.RemoveRange(_db.Apps);
            _db.Users.RemoveRange(_db.Users);
            _db.SaveChanges();

            SeedUsersToDb();
            SeedAppsToDb();
        }

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

        private void SeedAppsToDb()
        {
            foreach (var app in SeedApps())
                _db.Apps.Add(app);

            _db.SaveChanges();
        }

        private void SeedUsersToDb()
        {
            foreach (var user in SeedUsers())
                _db.Users.Add(user);

            _db.SaveChanges();
        }
    }
}
