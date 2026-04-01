using System;
using System.Windows;

namespace Project.Services
{
    public class LocalizationService : ILocalizationService
    {
        /// <summary>
        /// Swaps the language ResourceDictionary at index 1 of
        /// Application.Current.Resources.MergedDictionaries.
        /// Files are located at Resources/Localization/{langCode}.xaml
        /// </summary>
        public void SetLanguage(string langCode)
        {
            var dicts = Application.Current.Resources.MergedDictionaries;
            if (dicts.Count < 2)
                return;

            var uri = new Uri($"Resources/Localization/{langCode}.xaml", UriKind.Relative);

            dicts[1] = new ResourceDictionary { Source = uri };
        }
    }
}
