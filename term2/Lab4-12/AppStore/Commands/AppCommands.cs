using System.Windows.Input;

namespace Project.Commands
{
    public static class AppCommands
    {
        public static readonly RoutedUICommand DownloadApp = new RoutedUICommand(
            text: "Download App",
            name: "DownloadApp",
            ownerType: typeof(AppCommands)
        );

        public static readonly RoutedUICommand OpenDetails = new RoutedUICommand(
            text: "Open Details",
            name: "OpenDetails",
            ownerType: typeof(AppCommands)
        );
    }
}
