using System;
using System.Collections.Generic;
using System.IO;
using AppStore.Models;
using Newtonsoft.Json;

namespace AppStore.Data
{
    public interface IStorageService
    {
        List<AppItem> LoadApps();
        void SaveApps(List<AppItem> apps);
        void AddApp(AppItem app);
        void UpdateApp(AppItem app);
        void DeleteApp(Guid id);
        void DownloadApp(Guid id);
        void UninstallApp(Guid id);
        void RestoreDefaults();
    }

    public class JsonStorageService : IStorageService
    {
        private readonly string _filePath;
        private static readonly JsonSerializerSettings _settings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        public JsonStorageService(string? filePath = null)
        {
            _filePath =
                filePath
                ?? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "AppStore",
                    "apps.json"
                );

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            if (!File.Exists(_filePath))
                SaveApps(GetSeedData());
        }

        public List<AppItem> LoadApps()
        {
            if (!File.Exists(_filePath))
                return GetSeedData();
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<AppItem>>(json) ?? new();
        }

        public void SaveApps(List<AppItem> apps)
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(apps, _settings));
        }

        public void AddApp(AppItem app)
        {
            var apps = LoadApps();
            apps.Add(app);
            SaveApps(apps);
        }

        public void UpdateApp(AppItem app)
        {
            var apps = LoadApps();
            var idx = apps.FindIndex(a => a.Id == app.Id);
            if (idx >= 0)
                apps[idx] = app;
            SaveApps(apps);
        }

        public void DeleteApp(Guid id)
        {
            var apps = LoadApps();
            apps.RemoveAll(a => a.Id == id);
            SaveApps(apps);
        }

        public void DownloadApp(Guid id)
        {
            var apps = LoadApps();
            var app = apps.Find(a => a.Id == id);
            if (app != null)
            {
                app.IsDownloaded = true;
                app.DownloadCount++;
            }
            SaveApps(apps);
        }

        public void UninstallApp(Guid id)
        {
            var apps = LoadApps();
            var app = apps.Find(a => a.Id == id);
            if (app != null)
                app.IsDownloaded = false;
            SaveApps(apps);
        }

        public void RestoreDefaults()
        {
            SaveApps(GetSeedData());
        }

        private static List<AppItem> GetSeedData() =>
            new()
            {
                new AppItem
                {
                    ShortName = "Xcode",
                    FullName = "Xcode — IDE for Apple",
                    Description =
                        "Xcode is the complete developer toolset for creating apps for Apple platforms: iOS, macOS, watchOS, and tvOS.",
                    Developer = "Apple Inc.",
                    Category = "Development",
                    Rating = 3.5,
                    RatingCount = 18420,
                    Price = 0,
                    Version = "15.2",
                    SizeMB = 7200,
                    Country = "USA",
                    Color = "#1C1C1E",
                    IsFeatured = true,
                    AgeRating = "4+",
                    Tags = new() { "IDE", "Swift", "Apple", "Development" },
                    DownloadCount = 5200000,
                    ReleaseDate = new DateTime(2010, 6, 7),
                },
                new AppItem
                {
                    ShortName = "Figma",
                    FullName = "Figma: Collaborative Design",
                    Description =
                        "Figma is a powerful design platform for teams who build products together. Born on the Web, Figma helps teams create, test, and ship better designs from start to finish.",
                    Developer = "Figma, Inc.",
                    Category = "Design",
                    Rating = 4.8,
                    RatingCount = 32100,
                    Price = 0,
                    Version = "116.8",
                    SizeMB = 184,
                    Country = "USA",
                    Color = "#F24E1E",
                    IsFeatured = true,
                    AgeRating = "4+",
                    Tags = new() { "Design", "UI/UX", "Prototype", "Collaboration" },
                    DownloadCount = 8900000,
                    ReleaseDate = new DateTime(2016, 9, 27),
                },
                new AppItem
                {
                    ShortName = "Notion",
                    FullName = "Notion — Notes & Docs",
                    Description =
                        "Notion is the all-in-one workspace that combines notes, tasks, wikis, and databases. A new tool that blends your everyday work apps into one.",
                    Developer = "Notion Labs, Inc.",
                    Category = "Productivity",
                    Rating = 4.7,
                    RatingCount = 41250,
                    Price = 0,
                    Version = "3.1",
                    SizeMB = 210,
                    Country = "USA",
                    Color = "#000000",
                    IsFeatured = true,
                    AgeRating = "4+",
                    Tags = new() { "Notes", "Productivity", "Database", "Wiki" },
                    DownloadCount = 15000000,
                    ReleaseDate = new DateTime(2018, 3, 1),
                },
                new AppItem
                {
                    ShortName = "VS Code",
                    FullName = "Visual Studio Code",
                    Description =
                        "Visual Studio Code is a lightweight but powerful source code editor which runs on your desktop and is available for Windows, macOS and Linux.",
                    Developer = "Microsoft Corporation",
                    Category = "Development",
                    Rating = 4.9,
                    RatingCount = 87300,
                    Price = 0,
                    Version = "1.86",
                    SizeMB = 380,
                    Country = "USA",
                    Color = "#007ACC",
                    IsFeatured = false,
                    AgeRating = "4+",
                    Tags = new() { "Editor", "Code", "Microsoft", "Extensions" },
                    DownloadCount = 50000000,
                    ReleaseDate = new DateTime(2015, 4, 29),
                },
                new AppItem
                {
                    ShortName = "Slack",
                    FullName = "Slack — Business Communication",
                    Description =
                        "Slack is a new way to communicate with your team. It's faster, better organised and more secure than email.",
                    Developer = "Slack Technologies",
                    Category = "Social",
                    Rating = 4.3,
                    RatingCount = 29800,
                    Price = 0,
                    Version = "4.35",
                    SizeMB = 315,
                    Country = "USA",
                    Color = "#4A154B",
                    IsFeatured = false,
                    AgeRating = "4+",
                    Tags = new() { "Chat", "Teams", "Business", "Messaging" },
                    DownloadCount = 35000000,
                    ReleaseDate = new DateTime(2013, 8, 1),
                },
                new AppItem
                {
                    ShortName = "Spotify",
                    FullName = "Spotify — Music & Podcasts",
                    Description =
                        "With Spotify, you can play millions of songs and podcasts for free. Listen to the music and content you love and find new music to discover.",
                    Developer = "Spotify AB",
                    Category = "Media",
                    Rating = 4.6,
                    RatingCount = 112400,
                    Price = 0,
                    Version = "1.2.22",
                    SizeMB = 210,
                    Country = "Sweden",
                    Color = "#1DB954",
                    IsFeatured = true,
                    AgeRating = "12+",
                    Tags = new() { "Music", "Podcasts", "Streaming", "Entertainment" },
                    DownloadCount = 600000000,
                    ReleaseDate = new DateTime(2008, 10, 7),
                },
                new AppItem
                {
                    ShortName = "DaVinci Resolve",
                    FullName = "DaVinci Resolve — Video Editor",
                    Description =
                        "DaVinci Resolve is the world's only solution that combines professional 8K editing, color correction, visual effects and audio post production all in one software tool.",
                    Developer = "Blackmagic Design",
                    Category = "Media",
                    Rating = 4.7,
                    RatingCount = 22100,
                    Price = 0,
                    Version = "18.6",
                    SizeMB = 2800,
                    Country = "Australia",
                    Color = "#FF3B30",
                    IsFeatured = false,
                    AgeRating = "4+",
                    Tags = new() { "Video", "Edit", "Color", "Professional" },
                    DownloadCount = 3000000,
                    ReleaseDate = new DateTime(2004, 1, 1),
                },
                new AppItem
                {
                    ShortName = "1Password",
                    FullName = "1Password — Password Manager",
                    Description =
                        "1Password is a password manager that keeps you safe online. Save logins, passwords, credit cards, and more.",
                    Developer = "AgileBits Inc.",
                    Category = "Utilities",
                    Rating = 4.8,
                    RatingCount = 55300,
                    Price = 2.99,
                    Version = "8.10",
                    SizeMB = 95,
                    Country = "Canada",
                    Color = "#0094F5",
                    IsFeatured = false,
                    AgeRating = "4+",
                    Tags = new() { "Security", "Passwords", "Privacy" },
                    DownloadCount = 10000000,
                    ReleaseDate = new DateTime(2006, 6, 18),
                    DiscountPercent = 20,
                },
                new AppItem
                {
                    ShortName = "Minecraft",
                    FullName = "Minecraft",
                    Description =
                        "Explore infinite worlds and build everything from the simplest of homes to the grandest of castles. Play in creative mode with unlimited resources or mine deep into the world in survival mode.",
                    Developer = "Mojang Studios",
                    Category = "Games",
                    Rating = 4.5,
                    RatingCount = 890000,
                    Price = 6.99,
                    Version = "1.21",
                    SizeMB = 1200,
                    Country = "Sweden",
                    Color = "#62B547",
                    IsFeatured = true,
                    AgeRating = "7+",
                    Tags = new() { "Sandbox", "Adventure", "Building", "Multiplayer" },
                    DownloadCount = 300000000,
                    ReleaseDate = new DateTime(2011, 11, 18),
                },
                new AppItem
                {
                    ShortName = "Telegram",
                    FullName = "Telegram Messenger",
                    Description =
                        "Telegram is a cloud-based mobile and desktop messaging app with a focus on security and speed. Fast, secure, and feature-rich messaging.",
                    Developer = "Telegram FZ-LLC",
                    Category = "Social",
                    Rating = 4.8,
                    RatingCount = 198000,
                    Price = 0,
                    Version = "10.6",
                    SizeMB = 145,
                    Country = "UAE",
                    Color = "#2AABEE",
                    IsFeatured = false,
                    AgeRating = "4+",
                    Tags = new() { "Messaging", "Secure", "Channels", "Bots" },
                    DownloadCount = 800000000,
                    ReleaseDate = new DateTime(2013, 8, 14),
                },
                new AppItem
                {
                    ShortName = "Sketch",
                    FullName = "Sketch — Design Toolkit",
                    Description =
                        "Sketch is a design toolkit built to help you create your best work — from your earliest ideas, through to final artwork.",
                    Developer = "Sketch B.V.",
                    Category = "Design",
                    Rating = 4.4,
                    RatingCount = 14200,
                    Price = 9.99,
                    Version = "97.1",
                    SizeMB = 170,
                    Country = "Netherlands",
                    Color = "#F7B500",
                    IsFeatured = false,
                    AgeRating = "4+",
                    Tags = new() { "Design", "UI", "macOS", "Vector" },
                    DownloadCount = 1200000,
                    ReleaseDate = new DateTime(2010, 9, 7),
                },
                new AppItem
                {
                    ShortName = "Obsidian",
                    FullName = "Obsidian — Knowledge Base",
                    Description =
                        "Obsidian is a powerful knowledge base on top of a local folder of plain text Markdown files. It helps you think and connect ideas.",
                    Developer = "Dynalist Inc.",
                    Category = "Productivity",
                    Rating = 4.9,
                    RatingCount = 31000,
                    Price = 0,
                    Version = "1.5.3",
                    SizeMB = 210,
                    Country = "Canada",
                    Color = "#7C3AED",
                    IsFeatured = false,
                    AgeRating = "4+",
                    Tags = new() { "Notes", "Markdown", "Knowledge", "Zettelkasten" },
                    DownloadCount = 4000000,
                    ReleaseDate = new DateTime(2020, 3, 1),
                },
            };
    }
}
