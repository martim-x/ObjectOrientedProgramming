using System;

namespace Project.Models
{
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

        // Computed: first character of Login, uppercase
        string AvatarLetter { get; }
    }
}
