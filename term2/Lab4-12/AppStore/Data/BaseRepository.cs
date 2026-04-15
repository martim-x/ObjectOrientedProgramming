using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Project.Data
{
    /// <summary>
    /// Абстрактный базовый репозиторий.
    /// Реализует IRepository — все методы abstract, подклассы ОБЯЗАНЫ их override-нуть.
    /// Содержит общие Seed-данные и HashPassword — дублирование устранено.
    /// </summary>
    public abstract class BaseRepository : IRepository
    {
        // ── Абстрактные методы — Apps ───────────────────────────────────────────

        public abstract List<App> GetAllApps();
        public abstract App? GetAppById(Guid id);
        public abstract void AddApp(App app);
        public abstract void UpdateApp(App app);
        public abstract void DeleteApp(Guid id);
        public abstract void DownloadApp(Guid id);
        public abstract void UninstallApp(Guid id);
        public abstract void RestoreDefaults();

        // ── Абстрактные методы — Users ──────────────────────────────────────────

        public abstract List<User> GetAllUsers();
        public abstract User? GetUserByLogin(string login);
        public abstract void AddUser(User user);
        public abstract void UpdateUser(User user);

        // ── Общие утилиты ────────────────────────────────────────────────────────

        protected static string HashPassword(string password) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))).ToLower();

        // ── Seed — Apps ──────────────────────────────────────────────────────────

        protected static List<App> SeedApps() =>
            new()
            {
                new App
                {
                    Id = Guid.Parse("e7b8991e-9449-40d0-a576-c43a7a761a70"),
                    ShortName = "Figma",
                    FullName = "Figma \u2013 Design & Prototype",
                    Description =
                        "Figma is a collaborative interface design tool used by teams around the world.",
                    Developer = "Figma, Inc.",
                    Category = "Design",
                    Rating = 4.8,
                    RatingCount = 125430,
                    Price = 0,
                    Version = "116.7.0",
                    SizeMB = 186.3,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#F24E1E",
                    IsFeatured = true,
                    IsInStock = true,
                    DownloadCount = 5200000,
                    ReleaseDate = new DateTime(2016, 9, 27),
                    Tags = new() { "Design", "Prototyping", "Collaboration", "UI" },
                },
                new App
                {
                    Id = Guid.Parse("60f93d8d-8fcf-42eb-a54d-0fd84e16602a"),
                    ShortName = "Slack",
                    FullName = "Slack \u2013 Business Communication",
                    Description =
                        "Slack is a messaging app for business that connects people to the information they need.",
                    Developer = "Slack Technologies, LLC",
                    Category = "Productivity",
                    Rating = 4.7,
                    RatingCount = 980500,
                    Price = 0,
                    Version = "4.35.126",
                    SizeMB = 271.4,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#4A154B",
                    IsFeatured = true,
                    IsInStock = true,
                    DownloadCount = 12000000,
                    ReleaseDate = new DateTime(2013, 8, 14),
                    Tags = new() { "Messaging", "Teams", "Productivity", "Business" },
                },
                new App
                {
                    Id = Guid.Parse("c1b8015f-1bc0-4b78-8112-ce391eceee54"),
                    ShortName = "Xcode",
                    FullName = "Xcode \u2013 Apple Developer IDE",
                    Description =
                        "Xcode is Apple's integrated development environment for macOS, iOS, watchOS, and tvOS.",
                    Developer = "Apple",
                    Category = "Development",
                    Rating = 3.9,
                    RatingCount = 340200,
                    Price = 0,
                    Version = "15.3",
                    SizeMB = 12800,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#1575F9",
                    IsFeatured = false,
                    IsInStock = true,
                    DownloadCount = 8000000,
                    ReleaseDate = new DateTime(2003, 10, 24),
                    Tags = new() { "IDE", "Apple", "Swift", "Development" },
                },
                new App
                {
                    Id = Guid.Parse("ecb14f17-0ea4-41ff-a59d-7c6a2a409004"),
                    ShortName = "Photoshop",
                    FullName = "Adobe Photoshop",
                    Description =
                        "Adobe Photoshop is a raster graphics editor for professionals and enthusiasts alike.",
                    Developer = "Adobe Inc.",
                    Category = "Design",
                    Rating = 4.6,
                    RatingCount = 620000,
                    Price = 54.99,
                    Version = "25.5",
                    SizeMB = 3200,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#31A8FF",
                    IsFeatured = true,
                    IsInStock = true,
                    DownloadCount = 9500000,
                    DiscountPercent = 20,
                    ReleaseDate = new DateTime(1990, 2, 19),
                    Tags = new() { "Design", "Photo Editing", "Creative", "Adobe" },
                },
                new App
                {
                    Id = Guid.Parse("1e3b0e67-c70d-47ff-ba5b-f65540239a9f"),
                    ShortName = "Notion",
                    FullName = "Notion \u2013 Notes & Workspace",
                    Description =
                        "Notion is an all-in-one workspace for notes, tasks, wikis, and databases.",
                    Developer = "Notion Labs, Inc.",
                    Category = "Productivity",
                    Rating = 4.5,
                    RatingCount = 450300,
                    Price = 0,
                    Version = "3.0.1",
                    SizeMB = 142.8,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#000000",
                    IsFeatured = false,
                    IsInStock = true,
                    DownloadCount = 7800000,
                    ReleaseDate = new DateTime(2016, 6, 1),
                    Tags = new() { "Notes", "Productivity", "Wiki", "Database" },
                },
                new App
                {
                    Id = Guid.Parse("63ad3a22-fc71-4724-a8a9-5101e15376b5"),
                    ShortName = "Discord",
                    FullName = "Discord \u2013 Talk, Chat & Hang Out",
                    Description = "Discord is the easiest way to talk over voice, video, and text.",
                    Developer = "Discord Inc.",
                    Category = "Social",
                    Rating = 4.6,
                    RatingCount = 1200000,
                    Price = 0,
                    Version = "0.0.314",
                    SizeMB = 309.5,
                    Country = "US",
                    AgeRating = "17+",
                    Color = "#5865F2",
                    IsFeatured = true,
                    IsInStock = true,
                    DownloadCount = 20000000,
                    ReleaseDate = new DateTime(2015, 5, 13),
                    Tags = new() { "Chat", "Gaming", "Voice", "Social" },
                },
                new App
                {
                    Id = Guid.Parse("df07dc9b-e24b-44e3-9fb1-3144737547f0"),
                    ShortName = "VS Code",
                    FullName = "Visual Studio Code",
                    Description =
                        "Visual Studio Code is a lightweight but powerful source code editor.",
                    Developer = "Microsoft Corporation",
                    Category = "Development",
                    Rating = 4.9,
                    RatingCount = 890000,
                    Price = 0,
                    Version = "1.88.0",
                    SizeMB = 198.7,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#007ACC",
                    IsFeatured = true,
                    IsInStock = true,
                    DownloadCount = 25000000,
                    ReleaseDate = new DateTime(2015, 4, 29),
                    Tags = new() { "IDE", "Editor", "Development", "Microsoft" },
                },
                new App
                {
                    Id = Guid.Parse("e5ee2bdf-b6a8-4eab-8590-44ede8f7d678"),
                    ShortName = "Final Cut Pro",
                    FullName = "Final Cut Pro",
                    Description =
                        "Final Cut Pro is a professional video editing software by Apple.",
                    Developer = "Apple",
                    Category = "Media",
                    Rating = 4.7,
                    RatingCount = 280000,
                    Price = 299.99,
                    Version = "10.7.0",
                    SizeMB = 3800,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#333333",
                    IsFeatured = false,
                    IsInStock = true,
                    DownloadCount = 2100000,
                    ReleaseDate = new DateTime(2011, 6, 21),
                    Tags = new() { "Video", "Editing", "Professional", "Apple" },
                },
                new App
                {
                    Id = Guid.Parse("99862caf-93c4-41a4-9055-7a1af9c9726e"),
                    ShortName = "Logic Pro",
                    FullName = "Logic Pro",
                    Description =
                        "Logic Pro is a full-featured professional recording studio on the Mac.",
                    Developer = "Apple",
                    Category = "Media",
                    Rating = 4.8,
                    RatingCount = 195000,
                    Price = 199.99,
                    Version = "11.0",
                    SizeMB = 1200,
                    Country = "US",
                    AgeRating = "4+",
                    Color = "#FF6B00",
                    IsFeatured = false,
                    IsInStock = true,
                    DownloadCount = 1800000,
                    ReleaseDate = new DateTime(1993, 1, 1),
                    Tags = new() { "Music", "Audio", "Production", "Apple" },
                },
                new App
                {
                    Id = Guid.Parse("81bf2563-391e-4b32-8955-863f98b20d65"),
                    ShortName = "Steam",
                    FullName = "Steam \u2013 Game Launcher",
                    Description = "Steam is a video game digital distribution service by Valve.",
                    Developer = "Valve Corporation",
                    Category = "Games",
                    Rating = 4.3,
                    RatingCount = 760000,
                    Price = 0,
                    Version = "3.0",
                    SizeMB = 347.2,
                    Country = "US",
                    AgeRating = "17+",
                    Color = "#1B2838",
                    IsFeatured = true,
                    IsInStock = true,
                    DownloadCount = 30000000,
                    ReleaseDate = new DateTime(2003, 9, 12),
                    Tags = new() { "Games", "Gaming", "Valve", "Store" },
                },
                new App
                {
                    Id = Guid.Parse("7f98ddcd-e009-4aaf-9354-94e5b7c7e0e9"),
                    ShortName = "Spotify",
                    FullName = "Spotify \u2013 Music & Podcasts",
                    Description = "Spotify is a digital music, podcast, and video service.",
                    Developer = "Spotify AB",
                    Category = "Media",
                    Rating = 4.4,
                    RatingCount = 2100000,
                    Price = 0,
                    Version = "1.2.32",
                    SizeMB = 231.5,
                    Country = "SE",
                    AgeRating = "4+",
                    Color = "#1DB954",
                    IsFeatured = true,
                    IsInStock = true,
                    DownloadCount = 50000000,
                    ReleaseDate = new DateTime(2008, 10, 7),
                    Tags = new() { "Music", "Podcasts", "Streaming", "Audio" },
                },
                new App
                {
                    Id = Guid.Parse("79f5b512-9182-43dd-93f5-c75553983367"),
                    ShortName = "Telegram",
                    FullName = "Telegram Messenger",
                    Description =
                        "Telegram is a cloud-based mobile and desktop messaging app with a focus on security.",
                    Developer = "Telegram FZ-LLC",
                    Category = "Social",
                    Rating = 4.7,
                    RatingCount = 1800000,
                    Price = 0,
                    Version = "10.9.2",
                    SizeMB = 98.4,
                    Country = "AE",
                    AgeRating = "4+",
                    Color = "#2AABEE",
                    IsFeatured = false,
                    IsInStock = true,
                    DownloadCount = 40000000,
                    ReleaseDate = new DateTime(2013, 8, 14),
                    Tags = new() { "Messaging", "Privacy", "Social", "Chat" },
                },
            };

        // ── Seed — Users ─────────────────────────────────────────────────────────

        protected static List<User> SeedUsers() =>
            new()
            {
                new User
                {
                    Id = Guid.Parse("4fb0e47a-3971-43db-9716-5df42e41e66a"),
                    Login = "admin",
                    PasswordHash = HashPassword("admin"),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@appstore.local",
                    Role = UserRole.Admin,
                    AvatarColor = "#FF3B30",
                },
                new User
                {
                    Id = Guid.Parse("414603a5-19f8-4920-8d61-1fb8384acfb6"),
                    Login = "user",
                    PasswordHash = HashPassword("1234"),
                    FirstName = "Regular",
                    LastName = "User",
                    Email = "user@appstore.local",
                    Role = UserRole.User,
                    AvatarColor = "#34C759",
                },
            };

        // ── Seed — UsersApps ─────────────────────────────────────────────────────────

        protected static List<UsersApps> SeedUsersApps() => new();
    }
}
