using System;
using System.Security.Cryptography;
using System.Text;
using Project.Data;
using Project.Models;

namespace Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository _repo;
        private User? _currentUser;

        public User? CurrentUser => _currentUser;
        public bool IsAdmin => _currentUser?.Role == UserRole.Admin;

        public AuthService(IRepository repo)
        {
            _repo = repo;
        }

        public User? Login(string login, string password)
        {
            var hash = HashPassword(password);
            var user = _repo.GetUserByLogin(login);
            if (user == null || user.PasswordHash != hash)
                return null;

            _currentUser = user;
            return _currentUser;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public void UpdateProfile(string? firstName, string? lastName, string? email)
        {
            if (_currentUser == null)
                throw new InvalidOperationException("No user is logged in.");

            _currentUser.FirstName = firstName;
            _currentUser.LastName = lastName;
            _currentUser.Email = email;
            _repo.UpdateUser(_currentUser);
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (_currentUser == null)
                throw new InvalidOperationException("No user is logged in.");

            var oldHash = HashPassword(oldPassword);
            if (_currentUser.PasswordHash != oldHash)
                throw new UnauthorizedAccessException("Old password is incorrect.");

            _currentUser.PasswordHash = HashPassword(newPassword);
            _repo.UpdateUser(_currentUser);
        }

        public string HashPassword(string password) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))).ToLower();
    }
}
