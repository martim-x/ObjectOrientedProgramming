using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Project.Data
{
    /// <summary>
    /// Репозиторий на базе JSON-файлов.
    /// Рабочие файлы: %AppData%/AppStore/apps.json и users.json.
    /// При первом запуске или пустых/отсутствующих файлах — данные берутся из Seed.
    /// </summary>
    public class JsonRep : BaseRepository
    {
        private readonly string _appsFilePath;
        private readonly string _usersFilePath;
        private readonly string _usersAppsFilePath;

        private List<App> _appsCache = new();
        private List<User> _usersCache = new();
        private List<UsersApps> _usersAppsCache = new();

        public JsonRep(
            string? appsFilePath = null,
            string? usersFilePath = null,
            string? usersAppsFilePath = null
        )
        {
            var appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AppStore"
            );

            _appsFilePath = appsFilePath ?? Path.Combine(appDataDir, "apps.json");
            _usersFilePath = usersFilePath ?? Path.Combine(appDataDir, "users.json");
            _usersAppsFilePath = usersAppsFilePath ?? Path.Combine(appDataDir, "users_apps.json");

            LoadApps();
            LoadUsers();
            LoadUsersApps();
            WriteBufferFiles();
        }

        // ── Apps ───────────────────────────────────────────────────────────────

        // public override List<App> GetAllApps() => new(_appsCache);

        // public override App? GetAppById(Guid id) => _appsCache.FirstOrDefault(a => a.Id == id);
        public override List<App> GetAllApps()
        {
            var apps = new List<App>(_appsCache);
            var userId = GetCurrentUserId();

            if (userId != null)
            {
                var installedIds = _usersAppsCache
                    .Where(x => x.UserId == userId.Value)
                    .Select(x => x.AppId)
                    .ToHashSet();

                foreach (var app in apps)
                    app.IsDownloaded = installedIds.Contains(app.Id);
            }
            else
            {
                foreach (var app in apps)
                    app.IsDownloaded = false;
            }

            return apps;
        }

        public override App? GetAppById(Guid id)
        {
            var app = _appsCache.FirstOrDefault(a => a.Id == id);
            if (app == null)
                return null;

            var userId = GetCurrentUserId();
            if (userId != null)
            {
                app.IsDownloaded = _usersAppsCache.Any(x =>
                    x.UserId == userId.Value && x.AppId == id
                );
            }
            else
            {
                app.IsDownloaded = false;
            }

            return app;
        }

        public override void AddApp(App app)
        {
            _appsCache.Add(app);
            SaveApps();
        }

        public override void UpdateApp(App app)
        {
            var idx = _appsCache.FindIndex(a => a.Id == app.Id);
            if (idx >= 0)
                _appsCache[idx] = app;
            SaveApps();
        }

        public override void DeleteApp(Guid id)
        {
            _appsCache.RemoveAll(a => a.Id == id);
            _usersAppsCache.RemoveAll(x => x.AppId == id);

            SaveApps();
            SaveUsersApps();
        }

        public override void DownloadApp(Guid appId)
        {
            var app = _appsCache.FirstOrDefault(a => a.Id == appId);
            if (app == null)
                return;

            var userId = GetCurrentUserId();
            if (userId == null)
                return;

            app.DownloadCount++;

            app.IsDownloaded = true;

            var existing = _usersAppsCache.FirstOrDefault(x =>
                x.UserId == userId.Value && x.AppId == appId
            );
            if (existing == null)
            {
                _usersAppsCache.Add(
                    new UsersApps
                    {
                        UserId = userId.Value,
                        AppId = appId,
                        InstalledAt = DateTime.UtcNow,
                    }
                );
            }
            else
            {
                existing.InstalledAt = DateTime.UtcNow;
            }

            SaveApps();
            SaveUsersApps();
        }

        public override void UninstallApp(Guid appId)
        {
            var app = _appsCache.FirstOrDefault(a => a.Id == appId);
            if (app == null)
                return;

            var userId = GetCurrentUserId();
            if (userId == null)
                return;

            if (app.DownloadCount > 0)
                app.DownloadCount--;

            app.IsDownloaded = false;

            _usersAppsCache.RemoveAll(x => x.UserId == userId.Value && x.AppId == appId);

            SaveApps();
            SaveUsersApps();
        }

        public override void RestoreDefaults()
        {
            _appsCache = SeedApps();
            _usersAppsCache = SeedUsersApps();

            SaveApps();
            SaveUsersApps();
        }

        // ── Users ──────────────────────────────────────────────────────────────
        private Guid? GetCurrentUserId()
        {
            return _usersCache.FirstOrDefault()?.Id;
        }

        public override List<User> GetAllUsers() => new(_usersCache);

        public override User? GetUserByLogin(string login) =>
            _usersCache.FirstOrDefault(u =>
                string.Equals(u.Login, login, StringComparison.OrdinalIgnoreCase)
            );

        public override void AddUser(User user)
        {
            _usersCache.Add(user);
            SaveUsers();
        }

        public override void UpdateUser(User user)
        {
            var idx = _usersCache.FindIndex(u => u.Id == user.Id);
            if (idx >= 0)
                _usersCache[idx] = user;
            SaveUsers();
        }

        // ── Загрузка / сохранение ──────────────────────────────────────────────

        private void LoadApps()
        {
            try
            {
                if (File.Exists(_appsFilePath))
                {
                    var json = File.ReadAllText(_appsFilePath).Trim();
                    if (!string.IsNullOrEmpty(json) && json != "[]")
                    {
                        _appsCache = JsonConvert.DeserializeObject<List<App>>(json) ?? SeedApps();
                        return;
                    }
                }
                _appsCache = SeedApps();
                SaveApps();
            }
            catch
            {
                // Повреждённый файл — тихо восстанавливаем из Seed
                _appsCache = SeedApps();
                SaveApps();
            }
        }

        private void LoadUsers()
        {
            try
            {
                if (File.Exists(_usersFilePath))
                {
                    var json = File.ReadAllText(_usersFilePath).Trim();
                    if (!string.IsNullOrEmpty(json) && json != "[]")
                    {
                        _usersCache =
                            JsonConvert.DeserializeObject<List<User>>(json) ?? SeedUsers();
                        return;
                    }
                }
                _usersCache = SeedUsers();
                SaveUsers();
            }
            catch
            {
                _usersCache = SeedUsers();
                SaveUsers();
            }
        }

        private void LoadUsersApps()
        {
            try
            {
                if (File.Exists(_usersAppsFilePath))
                {
                    var json = File.ReadAllText(_usersAppsFilePath).Trim();
                    if (!string.IsNullOrEmpty(json) && json != "[]")
                    {
                        _usersAppsCache =
                            JsonConvert.DeserializeObject<List<UsersApps>>(json)
                            ?? new List<UsersApps>();
                        return;
                    }
                }

                _usersAppsCache = new List<UsersApps>();
                SaveUsersApps();
            }
            catch
            {
                _usersAppsCache = new List<UsersApps>();
                SaveUsersApps();
            }
        }

        private void WriteBufferFiles()
        {
            try
            {
                var bufferDir = Path.Combine(AppContext.BaseDirectory, "Resources", "Buffer");
                Directory.CreateDirectory(bufferDir);

                File.WriteAllText(
                    Path.Combine(bufferDir, "apps.json"),
                    JsonConvert.SerializeObject(SeedApps(), Formatting.Indented)
                );
                File.WriteAllText(
                    Path.Combine(bufferDir, "users.json"),
                    JsonConvert.SerializeObject(SeedUsers(), Formatting.Indented)
                );
                File.WriteAllText(
                    Path.Combine(bufferDir, "users_apps.json"),
                    JsonConvert.SerializeObject(SeedUsersApps(), Formatting.Indented)
                );
            }
            catch { }
        }

        private void SaveApps()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_appsFilePath)!);
            File.WriteAllText(
                _appsFilePath,
                JsonConvert.SerializeObject(_appsCache, Formatting.Indented)
            );
        }

        private void SaveUsers()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_usersFilePath)!);
            File.WriteAllText(
                _usersFilePath,
                JsonConvert.SerializeObject(_usersCache, Formatting.Indented)
            );
        }

        private void SaveUsersApps()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_usersAppsFilePath)!);
            File.WriteAllText(
                _usersAppsFilePath,
                JsonConvert.SerializeObject(_usersAppsCache, Formatting.Indented)
            );
        }
    }
}
