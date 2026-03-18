using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AppStore.Commands;
using AppStore.Models;
using AppStore.Services;

namespace AppStore.ViewModels
{
    /// <summary>
    /// Root ViewModel. Owns all state visible to the main window:
    /// the current app list, filter settings, navigation state, and commands.
    /// </summary>
    public sealed class MainViewModel : ViewModelBase
    {
        // ── public so Views can open child windows that need it
        public readonly IAppService Service;
        private readonly ILocalizationService _localization;

        // Maps sidebar nav key → Category string used in Filter (null = no category filter)
        private static readonly Dictionary<string, string?> NavCategoryMap = new()
        {
            ["Discover"] = null,
            ["Create"] = "Design",
            ["Work"] = "Productivity",
            ["Play"] = "Games",
            ["Develop"] = "Development",
            ["Installed"] = null,
            ["Categories"] = null,
        };

        // ── backing fields ──────────────────────────────────────────────────
        private ObservableCollection<AppItem> _apps = new();
        private ObservableCollection<AppItem> _featuredApps = new();
        private ObservableCollection<string> _allTags = new();
        private AppItem? _selectedApp;
        private string _searchQuery = string.Empty;
        private string _selectedCategory = "Discover";
        private string? _selectedTag;
        private double _minPrice;
        private double _maxPrice = 1000;
        private double _minRating;
        private string _sortBy = "featured";
        private bool _downloadedOnly;
        private bool _isDetailOpen;
        private bool _isCategoryGridOpen;
        private string _currentLanguage = "en";
        private int _visibleCount = 6;

        // ── observable collections ──────────────────────────────────────────
        public ObservableCollection<AppItem> Apps
        {
            get => _apps;
            set => SetField(ref _apps, value);
        }
        public ObservableCollection<AppItem> FeaturedApps
        {
            get => _featuredApps;
            set => SetField(ref _featuredApps, value);
        }
        public ObservableCollection<string> AllTags
        {
            get => _allTags;
            set => SetField(ref _allTags, value);
        }

        // ── selected app ────────────────────────────────────────────────────
        public AppItem? SelectedApp
        {
            get => _selectedApp;
            set
            {
                SetField(ref _selectedApp, value);
                if (value != null)
                    IsDetailOpen = true;
            }
        }

        // ── filter / search state ───────────────────────────────────────────
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (!SetField(ref _searchQuery, value))
                    return;
                _visibleCount = 6;
                _selectedTag = null;
                RefreshList();
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (!SetField(ref _selectedCategory, value))
                    return;
                _selectedTag = null;
                _visibleCount = 6;
                _downloadedOnly = value == "Installed";
                RefreshList();
                OnPropertyChanged(nameof(SelectedCategoryLabel));
                OnPropertyChanged(nameof(IsFeaturedVisible));
            }
        }

        public double MinPrice
        {
            get => _minPrice;
            set
            {
                if (SetField(ref _minPrice, value))
                    RefreshList();
            }
        }
        public double MaxPrice
        {
            get => _maxPrice;
            set
            {
                if (SetField(ref _maxPrice, value))
                    RefreshList();
            }
        }
        public double MinRating
        {
            get => _minRating;
            set
            {
                if (SetField(ref _minRating, value))
                    RefreshList();
            }
        }
        public string SortBy
        {
            get => _sortBy;
            set
            {
                if (SetField(ref _sortBy, value))
                    RefreshList();
            }
        }
        public bool DownloadedOnly
        {
            get => _downloadedOnly;
            set
            {
                if (SetField(ref _downloadedOnly, value))
                    RefreshList();
            }
        }

        // ── navigation state ────────────────────────────────────────────────
        public bool IsDetailOpen
        {
            get => _isDetailOpen;
            set => SetField(ref _isDetailOpen, value);
        }
        public bool IsCategoryGridOpen
        {
            get => _isCategoryGridOpen;
            set => SetField(ref _isCategoryGridOpen, value);
        }
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set => SetField(ref _currentLanguage, value);
        }

        // ── computed ────────────────────────────────────────────────────────
        public string SelectedCategoryLabel => _selectedTag ?? _selectedCategory;
        public bool IsFeaturedVisible =>
            _selectedCategory == "Discover"
            && string.IsNullOrEmpty(_searchQuery)
            && _selectedTag == null;
        public bool IsEmpty => Apps.Count == 0;
        public int TotalCount => Service.GetTotalCount();
        public int DownloadedCount => Service.GetDownloadedCount();

        // ── commands ─────────────────────────────────────────────────────────
        public ICommand DownloadCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand CloseDetailCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand SelectTagCommand { get; }
        public ICommand SwitchLanguageCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleSeeAllCommand { get; }
        public ICommand RestoreDefaultsCommand { get; }

        // ── constructor ──────────────────────────────────────────────────────
        public MainViewModel(IAppService service, ILocalizationService localization)
        {
            Service = service;
            _localization = localization;

            DownloadCommand = new DownloadAppCommand(
                service,
                app =>
                {
                    Reload();
                    OnPropertyChanged(nameof(DownloadedCount));
                    // Refresh SelectedApp reference so detail panel updates immediately
                    if (app != null && _selectedApp?.Id == app.Id)
                    {
                        var fresh = service.GetById(app.Id);
                        bool wasOpen = _isDetailOpen;
                        _selectedApp = null;
                        OnPropertyChanged(nameof(SelectedApp));
                        _selectedApp = fresh;
                        _isDetailOpen = wasOpen;
                        OnPropertyChanged(nameof(SelectedApp));
                    }
                }
            );

            DeleteCommand = new DeleteAppCommand(
                service,
                () =>
                {
                    IsDetailOpen = false;
                    SelectedApp = null;
                    Reload();
                }
            );

            OpenDetailCommand = new RelayCommand<AppItem>(app => SelectedApp = app);
            CloseDetailCommand = new RelayCommand(() =>
            {
                IsDetailOpen = false;
                SelectedApp = null;
                IsCategoryGridOpen = false;
            });

            SelectCategoryCommand = new RelayCommand<string>(cat =>
            {
                if (cat == null)
                    return;
                _selectedTag = null;
                if (cat == "Categories")
                {
                    IsCategoryGridOpen = true;
                    IsDetailOpen = false;
                    _selectedCategory = "Categories";
                    _visibleCount = int.MaxValue;
                    RefreshList();
                    OnPropertyChanged(nameof(SelectedCategory));
                    OnPropertyChanged(nameof(SelectedCategoryLabel));
                    OnPropertyChanged(nameof(IsFeaturedVisible));
                }
                else
                {
                    IsCategoryGridOpen = false;
                    SelectedCategory = cat;
                }
            });

            SelectTagCommand = new RelayCommand<string>(tag =>
            {
                if (tag == null)
                    return;
                _selectedTag = tag;
                _visibleCount = int.MaxValue;
                IsCategoryGridOpen = true;
                IsDetailOpen = false;
                RefreshList();
                OnPropertyChanged(nameof(IsFeaturedVisible));
                OnPropertyChanged(nameof(SelectedCategoryLabel));
            });

            SwitchLanguageCommand = new RelayCommand<string>(lang =>
            {
                if (lang != null)
                    SwitchLanguage(lang);
            });
            ClearSearchCommand = new RelayCommand(() => SearchQuery = string.Empty);
            RefreshCommand = new RelayCommand(Reload);
            ToggleSeeAllCommand = new RelayCommand(() =>
            {
                _visibleCount = _visibleCount <= 6 ? int.MaxValue : 6;
                RefreshList();
            });
            RestoreDefaultsCommand = new RelayCommand(() =>
            {
                service.RestoreDefaults();
                Reload();
            });

            Reload();
        }

        // ── public reload (called after add/edit/delete) ─────────────────────
        public void Reload()
        {
            LoadAllTags();
            LoadFeatured();
            RefreshList();
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(DownloadedCount));
        }

        // ── private helpers ──────────────────────────────────────────────────
        private void LoadAllTags()
        {
            var tags = Service.GetAll().SelectMany(a => a.Tags).Distinct().OrderBy(t => t);
            AllTags = new ObservableCollection<string>(tags);
        }

        private void LoadFeatured() =>
            FeaturedApps = new ObservableCollection<AppItem>(Service.GetFeatured());

        private void RefreshList()
        {
            var result = BuildQuery();

            if (_visibleCount < int.MaxValue)
                result = result.Take(_visibleCount);

            Apps = new ObservableCollection<AppItem>(result);
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(IsFeaturedVisible));
        }

        private IEnumerable<AppItem> BuildQuery()
        {
            if (!string.IsNullOrWhiteSpace(_searchQuery))
                return Service.Search(_searchQuery);

            if (_selectedTag != null)
            {
                var tagged = Service.GetAll().Where(a => a.Tags.Contains(_selectedTag));
                return _sortBy == "name"
                    ? tagged.OrderBy(a => a.ShortName)
                    : tagged.OrderByDescending(a => a.Rating);
            }

            if (_selectedCategory == "Categories")
                return Service.GetAll().OrderBy(a => a.ShortName);

            NavCategoryMap.TryGetValue(_selectedCategory, out var dbCat);
            bool? dlOnly = _selectedCategory == "Installed" || _downloadedOnly ? true : null;

            return Service.Filter(
                dbCat,
                _minPrice > 0 ? _minPrice : null,
                _maxPrice < 1000 ? _maxPrice : null,
                _minRating > 0 ? _minRating : null,
                dlOnly,
                _sortBy
            );
        }

        private void SwitchLanguage(string lang)
        {
            _localization.SetLanguage(lang);
            CurrentLanguage = lang;
            // Rebuild collections so converters re-evaluate with new currency/labels
            Reload();
        }
    }
}
