using System;
using System.Windows;
using System.Windows.Media;
using Project.Models;
using Project.Services;
using Project.ViewModels;

namespace Project.Views
{
    public partial class ProfilePopup : Window
    {
        private readonly IAuthService _auth;
        public bool LoggedOut { get; private set; }

        public ProfilePopup(IAuthService auth, ThemeService theme, Window owner)
        {
            InitializeComponent();
            _auth = auth;
            Owner = owner;

            var user = auth.CurrentUser!;
            AvatarLetter.Text = user.AvatarLetter;
            LoginText.Text = user.Login;

            // Avatar background
            AvatarBorder.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(user.AvatarColor)
            );

            // Role badge
            if (user.Role == UserRole.Admin)
            {
                RoleText.Text = "Admin";
                RoleBadge.Background = new SolidColorBrush(Color.FromRgb(255, 59, 48));
            }
            else
            {
                RoleText.Text = "User";
                RoleBadge.Background = new SolidColorBrush(Color.FromRgb(0, 114, 247));
            }

            // Setup ViewModel
            var vm = new ProfileViewModel(auth, theme);
            vm.GetOldPassword = () => OldPasswordBox.Password;
            vm.GetNewPassword = () => NewPasswordBox.Password;
            DataContext = vm;

            PositionNearOwner();
        }

        private void PositionNearOwner()
        {
            if (Owner == null)
                return;
            Left = Owner.Left + 12;
            Top = Owner.Top + Owner.ActualHeight - 580;
            if (Top < Owner.Top)
                Top = Owner.Top + 20;
        }

        private void OnLogout(object sender, RoutedEventArgs e)
        {
            LoggedOut = true;
            _auth.Logout();
            Close();
        }

        private void OnDeactivated(object sender, System.EventArgs e)
        {
            // Don't close if user is typing
        }

        private void OnPasswordBoxGotFocus(object sender, RoutedEventArgs e)
        {
            // Highlight border
            if (sender == OldPasswordBox && OldPwBorder != null)
                OldPwBorder.BorderBrush = (Brush)FindResource("AccentBlueBrush");
            if (sender == NewPasswordBox && NewPwBorder != null)
                NewPwBorder.BorderBrush = (Brush)FindResource("AccentBlueBrush");
        }

        private void OnPasswordBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender == OldPasswordBox && OldPwBorder != null)
                OldPwBorder.BorderBrush = (Brush)FindResource("DividerBrush");
            if (sender == NewPasswordBox && NewPwBorder != null)
                NewPwBorder.BorderBrush = (Brush)FindResource("DividerBrush");
        }
    }
}
