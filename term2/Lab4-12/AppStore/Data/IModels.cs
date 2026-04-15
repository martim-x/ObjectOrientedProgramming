using System;
using System.Collections.Generic;

namespace Project.Data
{
    public enum UserRole
    {
        User = 0,
        Admin = 1,
    }

    public interface IApp
    {
        Guid Id { get; set; }
        string ShortName { get; set; }
        string FullName { get; set; }
        string Description { get; set; }
        string Developer { get; set; }
        string Category { get; set; }
        double Rating { get; set; }
        int RatingCount { get; set; }
        double Price { get; set; }
        string Version { get; set; }
        double SizeMB { get; set; }
        string Country { get; set; }
        string AgeRating { get; set; }
        string Color { get; set; }
        bool IsFeatured { get; set; }
        bool IsDownloaded { get; set; }
        bool IsInStock { get; set; }
        int DownloadCount { get; set; }
        double? DiscountPercent { get; set; }
        DateTime ReleaseDate { get; set; }
        List<string> Tags { get; set; }
        double FinalPrice { get; } // вычисляемое
        string ButtonLabel { get; } // вычисляемое
    }

    public interface IUser
    {
        Guid Id { get; set; }
        string Login { get; set; }
        string PasswordHash { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
        string? Email { get; set; }
        UserRole Role { get; set; }
        string AvatarColor { get; set; }
        string AvatarLetter { get; } // вычисляемое
    }

    public interface IUsersApps
    {
        public Guid UserId { get; set; }
        public Guid AppId { get; set; }
        public DateTime InstalledAt { get; set; } = DateTime.Now;
    }
}
