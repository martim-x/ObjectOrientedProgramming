using System;
using System.Linq;
using System.Windows;
using Project.ViewModels;

namespace Project.Views
{
    public partial class EditDeleteDialog : Window
    {
        private readonly MainViewModel _vm;

        public EditDeleteDialog(MainViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            AppNameText.Text = vm.SelectedApp?.FullName ?? "";
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            Close();
            if (_vm.SelectedApp == null)
                return;
            var dlg = new AddEditWindow(_vm.Repository, _vm.SelectedApp)
            {
                Owner = Application.Current.MainWindow,
            };
            if (dlg.ShowDialog() == true)
                _vm.RefreshCommand.Execute(null);
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            Close();
            if (_vm.SelectedApp == null)
                return;
            var dlg = new ConfirmDialog(
                (string)Application.Current.Resources["DeleteConfirmTitle"],
                (string)Application.Current.Resources["DeleteConfirmMsg"],
                (string)Application.Current.Resources["DeleteBtn"],
                isDanger: true
            )
            {
                Owner = Application.Current.MainWindow,
            };
            if (dlg.ShowDialog() == true)
                _vm.DeleteCommand.Execute(_vm.SelectedApp.Id);
        }

        private void OnCancel(object sender, RoutedEventArgs e) => Close();
    }
}
