using System.Windows;
using AppStore.Models;
using AppStore.Services;
using AppStore.ViewModels;

namespace AppStore.Views
{
    public partial class AddEditWindow : Window
    {
        public AddEditWindow(IAppService service, AppItem? existing = null)
        {
            InitializeComponent();
            var vm = new AddEditAppViewModel(service, existing);
            vm.OnSaved = () =>
            {
                DialogResult = true;
                Close();
            };
            vm.OnCancelled = () =>
            {
                DialogResult = false;
                Close();
            };
            DataContext = vm;

            Title = existing != null ? "Edit App" : "Add New App";
        }
    }
}
