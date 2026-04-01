using System;
using System.Windows.Input;
using Project.Commands;
using Project.Services;

namespace Project.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthService _auth;
        private readonly ThemeService _theme;

        private string? _firstName;
        private string? _lastName;
        private string? _email;
        private bool _isDarkMode;
        private string _statusMessage = string.Empty;

        public string? FirstName
        {
            get => _firstName;
            set => SetField(ref _firstName, value);
        }
        public string? LastName
        {
            get => _lastName;
            set => SetField(ref _lastName, value);
        }
        public string? Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => SetField(ref _isDarkMode, value);
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public ICommand SaveProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand ToggleThemeCommand { get; }

        // Delegates for password retrieval from code-behind (PasswordBox can't bind)
        public Func<string>? GetOldPassword { get; set; }
        public Func<string>? GetNewPassword { get; set; }

        public ProfileViewModel(IAuthService auth, ThemeService theme)
        {
            _auth = auth;
            _theme = theme;
            _isDarkMode = theme.IsDark;

            var user = auth.CurrentUser;
            if (user != null)
            {
                _firstName = user.FirstName;
                _lastName = user.LastName;
                _email = user.Email;
            }

            SaveProfileCommand = new RelayCommand(SaveProfile);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            ToggleThemeCommand = new RelayCommand(ToggleTheme);
        }

        private void SaveProfile()
        {
            try
            {
                _auth.UpdateProfile(FirstName, LastName, Email);
                StatusMessage = "Profile saved";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ChangePassword()
        {
            var oldPw = GetOldPassword?.Invoke() ?? "";
            var newPw = GetNewPassword?.Invoke() ?? "";

            if (string.IsNullOrEmpty(newPw))
            {
                StatusMessage = "New password cannot be empty";
                return;
            }

            try
            {
                _auth.ChangePassword(oldPw, newPw);
                StatusMessage = "Password changed";
            }
            catch (UnauthorizedAccessException)
            {
                StatusMessage = "Old password is incorrect";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ToggleTheme()
        {
            _theme.SetTheme(IsDarkMode);
        }
    }
}
