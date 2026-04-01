using System;
using System.Collections.Generic;
using Project.Models;

namespace Project.Data
{
    public interface IRepository
    {
        // Apps
        List<App> GetAllApps();
        App? GetAppById(Guid id);
        void AddApp(App app);
        void UpdateApp(App app);
        void DeleteApp(Guid id);
        void DownloadApp(Guid id);
        void UninstallApp(Guid id); // DownloadCount-- on uninstall
        void RestoreDefaults();

        // Users
        List<User> GetAllUsers();
        User? GetUserByLogin(string login);
        void AddUser(User user);
        void UpdateUser(User user);
    }
}
