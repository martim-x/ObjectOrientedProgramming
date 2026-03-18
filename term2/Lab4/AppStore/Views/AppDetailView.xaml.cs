using System.Windows;
using System.Windows.Controls;
using AppStore.ViewModels;

namespace AppStore.Views
{
    public partial class AppDetailView : UserControl
    {
        public AppDetailView() => InitializeComponent();

        private void OnEditDeleteClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainViewModel vm || vm.SelectedApp == null)
                return;
            var dlg = new EditDeleteDialog(vm) { Owner = Window.GetWindow(this) };
            dlg.ShowDialog();
        }
    }
}
