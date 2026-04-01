using Project.Models;

namespace Project.Services
{
    public interface IAuthService
    {
        User? CurrentUser { get; }
        bool IsAdmin { get; }

        User? Login(string login, string password);
        void Logout();
        void UpdateProfile(string? firstName, string? lastName, string? email);
        void ChangePassword(string oldPassword, string newPassword);
        string HashPassword(string password);
    }
}
