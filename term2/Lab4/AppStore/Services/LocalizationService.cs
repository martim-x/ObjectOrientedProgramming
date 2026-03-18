using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using AppStore.Converters;

namespace AppStore.Services
{
    public interface ILocalizationService
    {
        string CurrentLanguage { get; }
        void SetLanguage(string language);
        string Get(string key);
        event EventHandler? LanguageChanged;
    }

    public class LocalizationService : ILocalizationService
    {
        public string CurrentLanguage { get; private set; } = "en";
        public event EventHandler? LanguageChanged;

        public void SetLanguage(string language)
        {
            CurrentLanguage = language;

            // Switch currency & rate
            if (language == "ru")
            {
                AppGetLabelConverter.CurrentCurrency = "BYN";
                AppGetLabelConverter.ExchangeRate = 2.97;
            }
            else
            {
                AppGetLabelConverter.CurrentCurrency = "USD";
                AppGetLabelConverter.ExchangeRate = 1.0;
            }

            // Swap resource dictionary
            var dict = new ResourceDictionary
            {
                Source = new Uri(
                    $"pack://application:,,,/Resources/Localization/{language}.xaml",
                    UriKind.Absolute
                ),
            };
            var merged = Application.Current.Resources.MergedDictionaries;
            for (int i = merged.Count - 1; i >= 0; i--)
                if ((merged[i].Source?.ToString() ?? "").Contains("/Localization/"))
                    merged.RemoveAt(i);
            merged.Add(dict);

            // Update Get/Open button labels from new dictionary
            AppGetLabelConverter.GetLabel = Get("BtnGet");
            AppGetLabelConverter.OpenLabel = Get("BtnOpen");

            Thread.CurrentThread.CurrentCulture = new CultureInfo(
                language == "ru" ? "ru-RU" : "en-US"
            );
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public string Get(string key) =>
            Application.Current.Resources.Contains(key)
                ? Application.Current.Resources[key]?.ToString() ?? key
                : key;
    }
}
