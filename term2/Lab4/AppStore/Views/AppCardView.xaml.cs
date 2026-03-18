using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppStore.Models;

namespace AppStore.Views
{
    public partial class AppCardView : UserControl
    {
        public static readonly DependencyProperty DownloadCommandProperty =
            DependencyProperty.Register(
                nameof(DownloadCommand),
                typeof(ICommand),
                typeof(AppCardView)
            );

        public static readonly DependencyProperty OpenDetailCommandProperty =
            DependencyProperty.Register(
                nameof(OpenDetailCommand),
                typeof(ICommand),
                typeof(AppCardView)
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

        public AppCardView() => InitializeComponent();

        private void OnCardClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AppItem app)
                OpenDetailCommand?.Execute(app);
        }
    }
}
