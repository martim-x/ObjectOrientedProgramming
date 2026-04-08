using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Project.Converters
{
    // ── 1. BoolToVisibilityConverter ──────────────────────────────────────────
    /// <summary>bool: true → Visible, false → Collapsed</summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => value is Visibility v && v == Visibility.Visible;
    }

    // ── 2. InverseBoolToVisibilityConverter ───────────────────────────────────
    /// <summary>bool: true → Collapsed, false → Visible</summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => value is bool b && b ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => value is Visibility v && v == Visibility.Collapsed;
    }

    // ── 3. NullToVisibilityConverter ──────────────────────────────────────────
    /// <summary>null → Collapsed, не null → Visible</summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => value == null ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;
    }

    // ── 4. PriceConverter ─────────────────────────────────────────────────────
    /// <summary>
    /// Форматирует цену в строку в зависимости от текущего языка.
    /// Читает "CurrentLanguage" из Application.Current.Resources и выбирает формат валюты.
    /// Примеры: en → "$9.99", ru → "9,99 ₽", de → "9,99 €".
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double price)
                return string.Empty;

            if (price <= 0)
                return GetString("Free") ?? "Free";

            var lang = Application.Current?.Resources["CurrentLanguage"] as string ?? "en";
            return lang switch
            {
                "ru" => $"{price:F2} ₽".Replace('.', ','),
                "de" => $"{price:F2} €".Replace('.', ','),
                "fr" => $"{price:F2} €".Replace('.', ','),
                _ => $"${price:F2}",
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;

        private static string? GetString(string key)
        {
            try
            {
                return Application.Current?.Resources[key] as string;
            }
            catch
            {
                return null;
            }
        }
    }

    // ── 5. DownloadCountConverter ─────────────────────────────────────────────
    /// <summary>Форматирует большие числа: 1234 → "1.2K", 1234567 → "1.2M".</summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class DownloadCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long n = value switch
            {
                int i => i,
                long l => l,
                double d => (long)d,
                _ => 0,
            };

            return n switch
            {
                >= 1_000_000 => $"{n / 1_000_000.0:0.#}M",
                >= 1_000 => $"{n / 1_000.0:0.#}K",
                _ => n.ToString(),
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;
    }

    // ── 6. RatingToStarsConverter ─────────────────────────────────────────────
    /// <summary>Оценка 4.5 → "★★★★☆" (целые звёзды закрашенные, остаток — пустые).</summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double rating)
                return string.Empty;

            // Округляем до ближайшего целого в диапазоне [0;5]
            int full = (int)Math.Clamp(Math.Round(rating), 0, 5);
            int empty = 5 - full;

            return new string('★', full) + new string('☆', empty);
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;
    }

    // ── 7. SizeConverter ──────────────────────────────────────────────────────
    /// <summary>Возвращает размер в человекочитаемом виде: "186.3 MB" или "12.5 GB".</summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double sizeMB)
                return string.Empty;

            return sizeMB >= 1024 ? $"{sizeMB / 1024.0:0.#} GB" : $"{sizeMB:0.#} MB";
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;
    }

    // ── 8. StringToColorBrushConverter ───────────────────────────────────────
    /// <summary>Преобразует строку с цветом в формате "#RRGGBB" в SolidColorBrush.</summary>
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class StringToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string hex)
                return new SolidColorBrush(Colors.Transparent);

            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                return new SolidColorBrush(color);
            }
            catch
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;
    }

    // ── 9. DiscountVisibilityConverter ────────────────────────────────────────
    /// <summary>null или 0 → Collapsed; любое положительное значение → Visible.</summary>
    [ValueConversion(typeof(double?), typeof(Visibility))]
    public class DiscountVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return Visibility.Collapsed;
            if (value is double d && d > 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;
    }

    // ── 10. RoleNameConverter ─────────────────────────────────────────────────
    /// <summary>bool IsAdmin → "Admin" / "User".</summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class RoleNameConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => value is bool b && b ? "Admin" : "User";

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        ) => DependencyProperty.UnsetValue;
    }

    // ── 11. ButtonLabelConverter ──────────────────────────────────────────────
    /// <summary>
    /// Формирует подпись на кнопке в зависимости от:
    /// values[0] = IsDownloaded (bool) — приложение уже установлено;
    /// values[1] = Price (double/decimal/float) — цена;
    /// values[2] = CurrentLanguage ("en" | "ru" и т.п.) — текущий язык, триггер пересчёта.
    /// </summary>
    public class ButtonLabelConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            if (values.Length < 3)
                return "Get";

            bool isDownloaded = values[0] is bool b && b;
            double price = values[1] switch
            {
                double d => d,
                decimal dc => (double)dc,
                float f => f,
                _ => 0d,
            };
            string lang = values[2] as string ?? "en";

            // Уже установлено — показываем локализованную кнопку открытия
            if (isDownloaded)
            {
                // "Open" / "Открыть" из ресурсов (ResourceDictionary)
                return Application.Current.TryFindResource("BtnOpen") as string ?? "Open";
            }

            // Бесплатное приложение — локализованная подпись "Get"/"Установить"
            if (price <= 0)
                return Application.Current.TryFindResource("BtnGet") as string ?? "Get";

            // Платное приложение — форматируем цену в валюте текущего языка
            return lang == "ru"
                ? $"{price * 2.97:F0} BYN" // грубое преобразование из USD в BYN
                : $"${price:F2}"; // цена в долларах США
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture
        ) => throw new NotSupportedException();
    }
}
