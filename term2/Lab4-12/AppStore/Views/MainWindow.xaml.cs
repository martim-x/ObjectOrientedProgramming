using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Project.Commands;
using Project.Data;
using Project.ViewModels;

namespace Project.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel Vm => (MainViewModel)DataContext;

        public ObservableCollection<string> EventLog { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var uri = new Uri("pack://application:,,,/Resources/Cursors/arrow.cur");
                var stream = Application.GetResourceStream(uri)?.Stream;
                if (stream != null)
                    Cursor = new Cursor(stream);
            }
            catch { }
        }

        private void Log(string message)
        {
            EventLog.Add($"{DateTime.Now:HH:mm:ss} | {message}");
            Debug.WriteLine(message);
        }

        // =========================
        // RoutedEvent demo
        // =========================

        // FeaturedBannerView: Tunnel
        private void Banner_PreviewBannerClick(object sender, RoutedEventArgs e)
        {
            Log("PreviewBannerClick (Tunneling) -> MainWindow");
        }

        // FeaturedBannerView: Direct
        private void Banner_BannerLoaded(object sender, RoutedEventArgs e)
        {
            Log("BannerLoaded (Direct) -> MainWindow");
        }

        // FeaturedBannerView: Bubble
        private void Banner_BannerClicked(object sender, RoutedEventArgs e)
        {
            Log("BannerClicked (Bubbling) -> MainWindow");
        }

        // AppCardView: Tunnel
        private void AppCard_PreviewCardClick(object sender, RoutedEventArgs e)
        {
            Log("PreviewCardClick (Tunneling) -> MainWindow");
        }

        // AppCardView: Bubble
        private void AppCard_CardClicked(object sender, RoutedEventArgs e)
        {
            Log("CardClicked (Bubbling) -> MainWindow");
        }

        // AppRowView: Bubble
        private void AppRow_RowClicked(object sender, RoutedEventArgs e)
        {
            Log("AppRow RowClicked (Bubbling) -> MainWindow");
        }

        // =========================
        // RoutedUICommand demo
        // =========================

        private void DownloadApp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is App;
        }

        private void DownloadApp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is App app)
            {
                Log($"RoutedUICommand DownloadApp executed for: {app.FullName}");

                if (Vm.DownloadCommand.CanExecute(app))
                    Vm.DownloadCommand.Execute(app);
            }
        }

        private void OpenDetails_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is App;
        }

        private void OpenDetails_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is App app)
            {
                Log($"RoutedUICommand OpenDetails executed for: {app.FullName}");

                if (Vm.OpenDetailCommand.CanExecute(app))
                    Vm.OpenDetailCommand.Execute(app);
            }
        }

        // =========================
        // Existing code
        // =========================

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
            var dlg = new AddEditWindow(Vm.Repository) { Owner = this };

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
                Vm.RestoreDefaultsCommand.Execute(null);
        }

        private void OnProfileClick(object sender, RoutedEventArgs e)
        {
            var popup = new ProfilePopup(Vm.AuthService, Vm.ThemeService, this);
            popup.Show();

            popup.Closed += (_, __) =>
            {
                if (popup.LoggedOut)
                    Vm.LogoutCommand.Execute(null);
            };
        }

        private void OnGridAppClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is App app)
                Vm.OpenDetailCommand.Execute(app);
        }
    }
}
