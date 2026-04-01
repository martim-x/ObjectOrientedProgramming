using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Project.Data;
using Project.Services;
using Project.ViewModels;
using Project.Views;

namespace Project
{
    public partial class ApplicationApp : Application
    {
        // ThemeService is static so it persists across login/logout cycles
        private static readonly ThemeService _themeService = new();

        private const string IconPath = "Resources/Icons/app.ico";
        private const string CursorPath = "Resources/Cursors/arrow.cur";

        [STAThread]
        public static void Main()
        {
            var app = new ApplicationApp();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            base.OnStartup(e);
            ShowLogin();
        }

        public static void ShowLogin()
        {
            var repo = new JsonRep();
            // var repo = new PostgreSQLRep();
            var auth = new AuthService(repo);
            var loginWin = new LoginWindow(auth);

            if (loginWin.ShowDialog() != true)
            {
                Current.Shutdown();
                return;
            }

            OpenMainWindow(repo, auth);
        }

        private static void OpenMainWindow(IRepository repo, IAuthService auth)
        {
            var localization = new LocalizationService();
            var vm = new MainViewModel(repo, localization, auth, _themeService);

            var window = new MainWindow { DataContext = vm };

            // Handle logout: close current window, show login again
            vm.LogoutRequested += (_, __) =>
            {
                window.Close();
                ShowLogin();
            };

            window.Show();
        }
    }
}
