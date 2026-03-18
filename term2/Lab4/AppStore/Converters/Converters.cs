using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AppStore.Models;

namespace AppStore.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is bool b && b ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            v is Visibility vis && vis == Visibility.Visible;
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is bool b && b ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v == null ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class StringNotEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is string s && s.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // -------------------------------------------------------
    // GET/OPEN label for row buttons
    // Binds to the whole AppItem, returns localised string or price
    // -------------------------------------------------------
    public class AppGetLabelConverter : IValueConverter
    {
        public static string CurrentCurrency { get; set; } = "USD";
        public static double ExchangeRate { get; set; } = 1.0;
        public static string GetLabel { get; set; } = "Get";
        public static string OpenLabel { get; set; } = "Open";

        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is AppItem app)
            {
                if (app.IsDownloaded)
                    return OpenLabel;
                if (app.IsFree)
                    return GetLabel;
                double price = app.FinalPrice * ExchangeRate;
                string sym = CurrentCurrency == "BYN" ? "BYN " : "$";
                return $"{sym}{price:F2}";
            }
            return GetLabel;
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // -------------------------------------------------------
    // DlBtnText — binds to bool IsDownloaded, returns Get/Open
    // -------------------------------------------------------
    public class DownloadButtonTextConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is bool b && b ? AppGetLabelConverter.OpenLabel : AppGetLabelConverter.GetLabel;

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // -------------------------------------------------------
    // Price in info table (Free / $9.99 / 29.40 BYN)
    // -------------------------------------------------------
    public class PriceConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is double price)
            {
                if (price <= 0)
                    return "Free";
                double converted = price * AppGetLabelConverter.ExchangeRate;
                string sym = AppGetLabelConverter.CurrentCurrency == "BYN" ? "BYN " : "$";
                return $"{sym}{converted:F2}";
            }
            return "Free";
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // -------------------------------------------------------
    // Discount — show ONLY when price > 0 AND discount > 0
    // -------------------------------------------------------
    public class DiscountVisibilityConverter : IValueConverter
    {
        // parameter = Price value passed via MultiBinding or ConverterParameter
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is double d && d > 0 ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // DiscountVisible only when both Price > 0 AND Discount > 0
    public class DiscountWithPriceVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type t, object p, CultureInfo c)
        {
            if (values.Length < 2)
                return Visibility.Collapsed;

            double price = values[0] is double d ? d : 0;
            double discount = values[1] is double dv ? dv : 0;

            return price > 0 && discount > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class DownloadCountConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is int n)
            {
                if (n >= 1_000_000)
                    return $"{n / 1_000_000.0:F1}M";
                if (n >= 1_000)
                    return $"{n / 1000.0:F0}K";
                return n.ToString();
            }
            return "0";
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is double r)
            {
                int full = Math.Max(
                    0,
                    Math.Min(5, (int)Math.Round(r, MidpointRounding.AwayFromZero))
                );
                return new string('★', full) + new string('☆', 5 - full);
            }
            return "★★★★★";
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class SizeConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is double mb)
                return mb >= 1024 ? $"{mb / 1024.0:F1} GB" : $"{mb:F1} MB";
            return "—";
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class SizeNumberConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is double mb ? (mb >= 1024 ? $"{mb / 1024.0:F1}" : $"{mb:F0}") : "0";

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class SizeUnitConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is double mb && mb >= 1024 ? "GB" : "MB";

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class StringToColorBrushConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is string hex)
            {
                try
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
                }
                catch { }
            }
            return new SolidColorBrush(Color.FromRgb(0, 114, 247));
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    public class StockToColorConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) =>
            v is bool b && b
                ? new SolidColorBrush(Color.FromRgb(52, 120, 246))
                : new SolidColorBrush(Color.FromRgb(128, 128, 128));

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotImplementedException();
    }
}
