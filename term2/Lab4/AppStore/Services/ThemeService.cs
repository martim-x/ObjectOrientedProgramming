using System;
using System.Windows;

namespace Project.Services
{
    public class ThemeService
    {
        private bool _isDark;

        public bool IsDark => _isDark;

        /// <summary>
        /// Swaps the style/theme ResourceDictionary at index 0 of
        /// Application.Current.Resources.MergedDictionaries.
        /// Light theme: Resources/Styles/LightAppTheme.xaml
        /// Dark  theme: Resources/Styles/DarkAppTheme.xaml
        /// </summary>
        public void SetTheme(bool isDark)
        {
            _isDark = isDark;

            var dicts = Application.Current.Resources.MergedDictionaries;

            var themeUri = isDark
                ? "pack://application:,,,/Styles/DarkAppTheme.xaml"
                : "pack://application:,,,/Styles/LightAppTheme.xaml";

            dicts[0] = new ResourceDictionary { Source = new Uri(themeUri, UriKind.Absolute) };
        }
    }
}
