using System.Windows;
using AppStore.Data;
using AppStore.Services;
using AppStore.ViewModels;
using AppStore.Views;

namespace AppStore
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var storage = new JsonStorageService();
            var repo = new AppRepository(storage);
            var service = new AppService(repo);
            var localization = new LocalizationService();

            var vm = new MainViewModel(service, localization);
            var window = new MainWindow { DataContext = vm };
            window.Show();
        }
    }
}
