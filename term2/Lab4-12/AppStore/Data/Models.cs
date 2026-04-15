using System;
using System.Collections.Generic;

namespace Project.Data
{
    public class App : IApp
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public double Price { get; set; }
        public string Version { get; set; } = string.Empty;
        public double SizeMB { get; set; }
        public string Country { get; set; } = string.Empty;
        public string AgeRating { get; set; } = "4+";
        public string Color { get; set; } = "#007AFF";
        public bool IsFeatured { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsInStock { get; set; } = true;
        public int DownloadCount { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<string> Tags { get; set; } = new();

        public double FinalPrice =>
            DiscountPercent.HasValue ? Price * (1.0 - DiscountPercent.Value / 100.0) : Price;

        public string ButtonLabel =>
            IsDownloaded ? "Открыть"
            : Price == 0 ? "Загрузить"
            : $"${FinalPrice:F2}";
    }

    public class User : IUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public string AvatarColor { get; set; } = "#007AFF";

        public string AvatarLetter =>
            string.IsNullOrEmpty(Login) ? "?" : Login[0].ToString().ToUpper();
    }

    public class UsersApps : IUsersApps
    {
        public Guid UserId { get; set; }
        public Guid AppId { get; set; }
        public DateTime InstalledAt { get; set; }
    }
}
