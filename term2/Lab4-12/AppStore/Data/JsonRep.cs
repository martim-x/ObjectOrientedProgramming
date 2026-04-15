using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Project.Data
{
    public class JsonRep : BaseRepository
    {
        private readonly string _appsFilePath;
        private readonly string _usersFilePath;
        private readonly string _usersAppsFilePath;
        private Guid? _currentUserId;

        private List<App> _appsCache = new();
        private List<User> _usersCache = new();
        private List<UsersApps> _usersAppsCache = new();

        public JsonRep(
            Guid? currentUserId = null,
            string? appsFilePath = null,
            string? usersFilePath = null,
            string? usersAppsFilePath = null
        )
        {
            _currentUserId = currentUserId;

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
            SaveBufferFiles();
        }

        public void SetCurrentUser(Guid? userId)
        {
            _currentUserId = userId;
            SaveBufferFiles();
        }

        public override List<App> GetAllApps()
        {
            var apps = new List<App>(_appsCache);
            var userId = _currentUserId;

            if (userId == null)
            {
                foreach (var app in apps)
                    app.IsDownloaded = false;

                return apps;
            }

            var installedIds = _usersAppsCache
                .Where(x => x.UserId == userId.Value)
                .Select(x => x.AppId)
                .ToHashSet();

            foreach (var app in apps)
                app.IsDownloaded = installedIds.Contains(app.Id);

            return apps;
        }

        public override App? GetAppById(Guid id)
        {
            var app = _appsCache.FirstOrDefault(a => a.Id == id);
            if (app == null)
                return null;

            app.IsDownloaded =
                _currentUserId != null
                && _usersAppsCache.Any(x => x.UserId == _currentUserId.Value && x.AppId == id);

            return app;
        }

        public override void AddApp(App app)
        {
            _appsCache.Add(app);
            SaveApps();
            SaveBufferFiles();
        }

        public override void UpdateApp(App app)
        {
            var idx = _appsCache.FindIndex(a => a.Id == app.Id);
            if (idx >= 0)
                _appsCache[idx] = app;

            SaveApps();
            SaveBufferFiles();
        }

        public override void DeleteApp(Guid id)
        {
            _appsCache.RemoveAll(a => a.Id == id);
            _usersAppsCache.RemoveAll(x => x.AppId == id);

            SaveApps();
            SaveUsersApps();
            SaveBufferFiles();
        }

        public override void DownloadApp(Guid appId)
        {
            if (_currentUserId == null)
                return;

            var app = _appsCache.FirstOrDefault(a => a.Id == appId);
            if (app == null)
                return;

            var exists = _usersAppsCache.Any(x =>
                x.UserId == _currentUserId.Value && x.AppId == appId
            );

            if (!exists)
            {
                _usersAppsCache.Add(
                    new UsersApps
                    {
                        UserId = _currentUserId.Value,
                        AppId = appId,
                        InstalledAt = DateTime.UtcNow,
                    }
                );

                app.DownloadCount++;
                app.IsDownloaded = true;

                SaveApps();
                SaveUsersApps();
                SaveBufferFiles();
            }
        }

        public override void UninstallApp(Guid appId)
        {
            if (_currentUserId == null)
                return;

            var app = _appsCache.FirstOrDefault(a => a.Id == appId);
            if (app == null)
                return;

            var removed = _usersAppsCache.RemoveAll(x =>
                x.UserId == _currentUserId.Value && x.AppId == appId
            );

            if (removed > 0)
            {
                if (app.DownloadCount > 0)
                    app.DownloadCount--;

                app.IsDownloaded = false;

                SaveApps();
                SaveUsersApps();
                SaveBufferFiles();
            }
        }

        public override void RestoreDefaults()
        {
            _appsCache = SeedApps();
            _usersCache = SeedUsers();
            _usersAppsCache = SeedUsersApps();

            SaveApps();
            SaveUsers();
            SaveUsersApps();
            SaveBufferFiles();
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
            SaveBufferFiles();
        }

        public override void UpdateUser(User user)
        {
            var idx = _usersCache.FindIndex(u => u.Id == user.Id);
            if (idx >= 0)
                _usersCache[idx] = user;

            SaveUsers();
            SaveBufferFiles();
        }

        private void LoadApps()
        {
            try
            {
                if (File.Exists(_appsFilePath))
                {
                    var json = File.ReadAllText(_appsFilePath).Trim();
                    if (!string.IsNullOrWhiteSpace(json) && json != "[]")
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
                    if (!string.IsNullOrWhiteSpace(json) && json != "[]")
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
                    if (!string.IsNullOrWhiteSpace(json) && json != "[]")
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

        private void SaveBufferFiles()
        {
            try
            {
                var bufferDir = Path.Combine(AppContext.BaseDirectory, "Resources", "Buffer");
                Directory.CreateDirectory(bufferDir);

                File.WriteAllText(
                    Path.Combine(bufferDir, "apps.json"),
                    JsonConvert.SerializeObject(_appsCache, Formatting.Indented)
                );

                File.WriteAllText(
                    Path.Combine(bufferDir, "users.json"),
                    JsonConvert.SerializeObject(_usersCache, Formatting.Indented)
                );

                File.WriteAllText(
                    Path.Combine(bufferDir, "usersapps.json"),
                    JsonConvert.SerializeObject(_usersAppsCache, Formatting.Indented)
                );
            }
            catch { }
        }
    }
}
