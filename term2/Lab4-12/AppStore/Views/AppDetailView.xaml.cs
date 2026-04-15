using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Project.ViewModels;

namespace Project.Views
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
