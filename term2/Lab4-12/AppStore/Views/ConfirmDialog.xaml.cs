using System;
using System.Windows;
using System.Windows.Media;

namespace Project.Views
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialog(
            string title,
            string message,
            string confirmLabel,
            bool isDanger = false
        )
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;
            ConfirmBtn.Content = confirmLabel;
            ConfirmBtn.Background = new SolidColorBrush(
                isDanger ? Color.FromRgb(255, 59, 48) : Color.FromRgb(0, 114, 247)
            );
        }

        private void OnConfirm(object s, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancel(object s, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
