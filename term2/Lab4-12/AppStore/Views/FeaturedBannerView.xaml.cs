using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Project.ViewModels;

namespace Project.Views
{
    public partial class FeaturedBannerView : UserControl
    {
        public FeaturedBannerView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            bool isDark = Application.Current.Resources["IsDarkTheme"] is bool b && b;

            Color startBase = isDark
                ? (Color)ColorConverter.ConvertFromString("#162447")
                : (Color)ColorConverter.ConvertFromString("#E8F4FD");

            Color startAlt = isDark
                ? (Color)ColorConverter.ConvertFromString("#1B335A")
                : (Color)ColorConverter.ConvertFromString("#D6EAFF");

            Color endBase = isDark
                ? (Color)ColorConverter.ConvertFromString("#1F4068")
                : (Color)ColorConverter.ConvertFromString("#D6EAFF");

            Color endAlt = isDark
                ? (Color)ColorConverter.ConvertFromString("#1B1B2F")
                : (Color)ColorConverter.ConvertFromString("#B8DBFF");

            GradStop0.Color = startBase;
            GradStop1.Color = endBase;

            var anim0 = new ColorAnimation
            {
                From = startBase,
                To = startAlt,
                Duration = TimeSpan.FromSeconds(4),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
            };

            var anim1 = new ColorAnimation
            {
                From = endBase,
                To = endAlt,
                Duration = TimeSpan.FromSeconds(4),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
            };

            GradStop0.BeginAnimation(GradientStop.ColorProperty, anim0);
            GradStop1.BeginAnimation(GradientStop.ColorProperty, anim1);
        }

        private void OnBannerClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.FeaturedApps.Count > 0)
                vm.OpenDetailCommand.Execute(vm.FeaturedApps[0]);
        }
    }
}
