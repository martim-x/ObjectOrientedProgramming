using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AppStore.ViewModels;

namespace AppStore.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel Vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var uri = new System.Uri("pack://application:,,,/Resources/Cursors/arrow.cur");
                var stream = Application.GetResourceStream(uri)?.Stream;
                if (stream != null)
                    Cursor = new Cursor(stream);
            }
            catch { }
        }

        private void OnSearchGotFocus(object sender, RoutedEventArgs e)
        {
            SearchBorder.BorderThickness = new Thickness(1.5);
            SearchBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("SearchFocusBrush");
        }

        private void OnSearchLostFocus(object sender, RoutedEventArgs e)
        {
            SearchBorder.BorderThickness = new Thickness(0);
        }

        private void OnClearSearch(object sender, MouseButtonEventArgs e)
        {
            Vm.ClearSearchCommand.Execute(null);
        }

        private void OnFilterClick(object sender, RoutedEventArgs e)
        {
            FilterPopup.PlacementTarget = (UIElement)sender;
            FilterPopup.Placement = PlacementMode.Bottom;
            FilterPopup.IsOpen = true;
        }

        private void OnAddAppClick(object sender, RoutedEventArgs e)
        {
            var dlg = new AddEditWindow(Vm.Service) { Owner = this };
            if (dlg.ShowDialog() == true)
                Vm.RefreshCommand.Execute(null);
        }

        private void OnResetFilters(object sender, RoutedEventArgs e)
        {
            Vm.MinRating = 0;
            Vm.MaxPrice = 1000;
            Vm.MinPrice = 0;
            Vm.DownloadedOnly = false;
            FilterPopup.IsOpen = false;
        }

        private void OnGridAppClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (
                sender is System.Windows.FrameworkElement fe
                && fe.DataContext is AppStore.Models.AppItem app
            )
                Vm.OpenDetailCommand.Execute(app);
        }

        private void OnRestoreData(object sender, RoutedEventArgs e)
        {
            FilterPopup.IsOpen = false;
            var dlg = new ConfirmDialog(
                (string)Application.Current.Resources["RestoreConfirmTitle"],
                (string)Application.Current.Resources["RestoreConfirmMsg"],
                (string)Application.Current.Resources["RestoreBtn"],
                isDanger: false
            )
            {
                Owner = this,
            };
            if (dlg.ShowDialog() == true)
            {
                Vm.RestoreDefaultsCommand.Execute(null);
            }
        }
    }
}
