using System;
using System.Diagnostics;
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
        // ---------- DependencyProperty: BannerTitle ----------
        public static readonly DependencyProperty BannerTitleProperty = DependencyProperty.Register(
            nameof(BannerTitle),
            typeof(string),
            typeof(FeaturedBannerView),
            new FrameworkPropertyMetadata(
                "Featured Today",
                FrameworkPropertyMetadataOptions.AffectsRender
            ),
            new ValidateValueCallback(ValidateBannerTitle)
        );

        public string BannerTitle
        {
            get => (string)GetValue(BannerTitleProperty);
            set => SetValue(BannerTitleProperty, value);
        }

        private static bool ValidateBannerTitle(object value)
        {
            var s = value as string;
            return !string.IsNullOrWhiteSpace(s);
        }

        // ---------- DependencyProperty: AnimationDurationSeconds ----------
        public static readonly DependencyProperty AnimationDurationSecondsProperty =
            DependencyProperty.Register(
                nameof(AnimationDurationSeconds),
                typeof(double),
                typeof(FeaturedBannerView),
                new FrameworkPropertyMetadata(
                    4.0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,
                    CoerceAnimationDuration
                ),
                ValidateAnimationDuration
            );

        public double AnimationDurationSeconds
        {
            get => (double)GetValue(AnimationDurationSecondsProperty);
            set => SetValue(AnimationDurationSecondsProperty, value);
        }

        private static bool ValidateAnimationDuration(object value)
        {
            if (value is double d)
                return d > 0 && !double.IsNaN(d) && !double.IsInfinity(d);
            return false;
        }

        private static object CoerceAnimationDuration(DependencyObject d, object baseValue)
        {
            if (baseValue is not double dVal)
                return 4.0;

            if (dVal < 1.0)
                return 1.0;
            if (dVal > 10.0)
                return 10.0;
            return dVal;
        }

        // ---------- RoutedEvents ----------

        // Tunnel (Preview)
        public static readonly RoutedEvent PreviewBannerClickEvent =
            EventManager.RegisterRoutedEvent(
                nameof(PreviewBannerClick),
                RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler),
                typeof(FeaturedBannerView)
            );

        public event RoutedEventHandler PreviewBannerClick
        {
            add => AddHandler(PreviewBannerClickEvent, value);
            remove => RemoveHandler(PreviewBannerClickEvent, value);
        }

        // Direct
        public static readonly RoutedEvent BannerLoadedEvent = EventManager.RegisterRoutedEvent(
            nameof(BannerLoaded),
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(FeaturedBannerView)
        );

        public event RoutedEventHandler BannerLoaded
        {
            add => AddHandler(BannerLoadedEvent, value);
            remove => RemoveHandler(BannerLoadedEvent, value);
        }

        // Bubble
        public static readonly RoutedEvent BannerClickedEvent = EventManager.RegisterRoutedEvent(
            nameof(BannerClicked),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(FeaturedBannerView)
        );

        public event RoutedEventHandler BannerClicked
        {
            add => AddHandler(BannerClickedEvent, value);
            remove => RemoveHandler(BannerClickedEvent, value);
        }

        public FeaturedBannerView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Direct event при загрузке
            RaiseEvent(new RoutedEventArgs(BannerLoadedEvent, this));

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

            var duration = TimeSpan.FromSeconds(AnimationDurationSeconds);

            var anim0 = new ColorAnimation
            {
                From = startBase,
                To = startAlt,
                Duration = duration,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
            };

            var anim1 = new ColorAnimation
            {
                From = endBase,
                To = endAlt,
                Duration = duration,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
            };

            GradStop0.BeginAnimation(GradientStop.ColorProperty, anim0);
            GradStop1.BeginAnimation(GradientStop.ColorProperty, anim1);
        }

        private void OnBannerClick(object sender, MouseButtonEventArgs e)
        {
            // Tunneling событие пойдёт сверху вниз
            RaiseEvent(new RoutedEventArgs(PreviewBannerClickEvent, this));

            if (DataContext is MainViewModel vm && vm.FeaturedApps.Count > 0)
                vm.OpenDetailCommand.Execute(vm.FeaturedApps[0]);

            // Bubbling событие пойдет снизу вверх
            RaiseEvent(new RoutedEventArgs(BannerClickedEvent, this));

            // Лог в Output (Debug)
            Debug.WriteLine("[FeaturedBannerView] Banner clicked");
        }
    }
}
