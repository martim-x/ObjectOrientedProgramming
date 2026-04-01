using System;
using System.Windows.Controls;
using System.Windows.Input;
using Project.ViewModels;

namespace Project.Views
{
    public partial class FeaturedBannerView : UserControl
    {
        public FeaturedBannerView() => InitializeComponent();

        private void OnBannerClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.FeaturedApps.Count > 0)
                vm.OpenDetailCommand.Execute(vm.FeaturedApps[0]);
        }
    }
}
