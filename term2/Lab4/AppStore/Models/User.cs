using System;

namespace Project.Models
{
    public enum UserRole
    {
        User = 0,
        Admin = 1,
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

        // Computed: first character of Login, uppercase
        public string AvatarLetter =>
            string.IsNullOrEmpty(Login) ? "?" : Login[0].ToString().ToUpper();
    }
}
