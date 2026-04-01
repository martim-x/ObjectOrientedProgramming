using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Project.Services;

namespace Project.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IAuthService _auth;

        public LoginWindow(IAuthService auth)
        {
            InitializeComponent();
            _auth = auth;
            LoginBox.Focus();
        }

        private void OnSignIn(object sender, RoutedEventArgs e) => TryLogin();

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TryLogin();
        }

        private void TryLogin()
        {
            var login = LoginBox.Text.Trim();
            var password = PassBox.Password;

            var user = _auth.Login(login, password);

            if (user != null)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                ErrorText.Text = (string)Application.Current.Resources["LoginError"];
                ErrorBorder.Visibility = Visibility.Visible;
                PassBox.Clear();

                LoginFieldBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 59, 48));
                PassFieldBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 59, 48));
            }
        }

        private void OnFieldFocus(object sender, RoutedEventArgs e)
        {
            // Reset red borders on focus
            LoginFieldBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 114, 247));
            PassFieldBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 114, 247));
            ErrorBorder.Visibility = Visibility.Collapsed;
        }

        private void OnFieldBlur(object sender, RoutedEventArgs e)
        {
            LoginFieldBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            PassFieldBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(230, 230, 230));
        }
    }
}
