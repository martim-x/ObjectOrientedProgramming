using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppStore.Models;

namespace AppStore.Views
{
    public partial class AppRowView : UserControl
    {
        public static readonly DependencyProperty DownloadCommandProperty =
            DependencyProperty.Register(
                nameof(DownloadCommand),
                typeof(ICommand),
                typeof(AppRowView)
            );
        public static readonly DependencyProperty OpenDetailCommandProperty =
            DependencyProperty.Register(
                nameof(OpenDetailCommand),
                typeof(ICommand),
                typeof(AppRowView)
            );

        public ICommand? DownloadCommand
        {
            get => (ICommand?)GetValue(DownloadCommandProperty);
            set => SetValue(DownloadCommandProperty, value);
        }
        public ICommand? OpenDetailCommand
        {
            get => (ICommand?)GetValue(OpenDetailCommandProperty);
            set => SetValue(OpenDetailCommandProperty, value);
        }

        public AppRowView() => InitializeComponent();

        private void OnRowClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AppItem app)
                OpenDetailCommand?.Execute(app);
        }
    }
}
