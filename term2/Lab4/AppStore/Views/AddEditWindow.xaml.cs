using System.Windows;
using Project.Data;
using Project.Models;
using Project.ViewModels;

namespace Project.Views
{
    public partial class AddEditWindow : Window
    {
        public AddEditWindow(IRepository repository, App? existing = null)
        {
            InitializeComponent();
            var vm = new AddEditAppViewModel(repository, existing);
            vm.OnSaved = () =>
            {
                DialogResult = true;
                Close();
            };
            vm.OnCancelled = () =>
            {
                DialogResult = false;
                Close();
            };
            DataContext = vm;

            Title = existing != null ? "Edit App" : "Add New App";
        }
    }
}
