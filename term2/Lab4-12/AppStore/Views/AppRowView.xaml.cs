using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Project.Data;

namespace Project.Views
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

        // ===== RoutedEvent: RowClicked (Bubble) =====

        public static readonly RoutedEvent RowClickedEvent = EventManager.RegisterRoutedEvent(
            nameof(RowClicked),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(AppRowView)
        );

        public event RoutedEventHandler RowClicked
        {
            add => AddHandler(RowClickedEvent, value);
            remove => RemoveHandler(RowClickedEvent, value);
        }

        public AppRowView()
        {
            InitializeComponent();
        }

        private void OnRowClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is App app)
                OpenDetailCommand?.Execute(app);

            // поднимаем bubbling-событие
            RaiseEvent(new RoutedEventArgs(RowClickedEvent, this));

            Debug.WriteLine("[AppRowView] Row clicked");
        }
    }
}
