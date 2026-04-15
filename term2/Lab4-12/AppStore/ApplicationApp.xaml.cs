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
        // Экземплярное поле — живёт весь цикл приложения, переживает logout
        private readonly IThemeService _themeService = new ThemeService();

        [STAThread]
        public static void Main()
        {
            var app = new ApplicationApp();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            base.OnStartup(e);

            var resourceInfo = Application.GetResourceStream(
                new Uri("Resources/Cursors/arrow.cur", UriKind.Relative)
            );
            if (resourceInfo != null)
            {
                var appCursor = new Cursor(resourceInfo.Stream);

                EventManager.RegisterClassHandler(
                    typeof(Window),
                    Window.LoadedEvent,
                    new RoutedEventHandler(
                        (sender, _) =>
                        {
                            if (sender is Window window)
                                window.Cursor = appCursor;
                        }
                    )
                );
            }
            this.ShowLogin();
        }

        private void ShowLogin()
        {
            // var repo = new JsonRep();
            var repo = new PostgreSQLRepEF();
            // var repo = new PostgreSQLRepADO();
            var auth = new AuthService(repo);
            var login = new LoginWindow(auth);

            // Закрыли логин без входа — завершаем процесс
            if (login.ShowDialog() != true)
            {
                this.Shutdown();
                return;
            }

            this.OpenCatalog(repo, auth);
        }

        // Открывает каталог; управляет двумя сценариями закрытия окна
        private void OpenCatalog(IRepository repo, IAuthService auth)
        {
            var vm = new MainViewModel(repo, new LocalizationService(), auth, this._themeService);
            var window = new MainWindow { DataContext = vm };
            var isLogout = false;

            // Закрываем каталог — завершаем процесс
            window.Closed += (_, __) =>
            {
                if (!isLogout)
                    this.Shutdown();
            };

            // Logout — закрываем каталог и возвращаемся к логину
            vm.LogoutRequested += (_, __) =>
            {
                isLogout = true; // ← до Close(), иначе Closed вызовет Shutdown
                window.Close();
                this.ShowLogin();
            };

            window.Show();
        }
    }
}
