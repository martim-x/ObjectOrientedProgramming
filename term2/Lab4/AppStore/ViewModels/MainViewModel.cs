using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Project.Commands;
using Project.Data;
using Project.Models;
using Project.Services;

namespace Project.ViewModels
{
    /// <summary>
    /// Root ViewModel. Owns all state visible to the main window:
    /// the current app list, filter settings, navigation state, and commands.
    /// Uses IRepository directly — no AppService layer.
    /// </summary>
    public sealed class MainViewModel : ViewModelBase
    {
        private readonly IRepository _repo;
        private readonly ILocalizationService _localization;
        private readonly IAuthService _auth;
        private readonly ThemeService _theme;

        // Public so Views can access for add/edit dialogs
        public IRepository Repository => _repo;

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
        private ObservableCollection<App> _apps = new();
        private ObservableCollection<App> _featuredApps = new();
        private ObservableCollection<string> _allTags = new();
        private App? _selectedApp;
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
        public ObservableCollection<App> Apps
        {
            get => _apps;
            set => SetField(ref _apps, value);
        }
        public ObservableCollection<App> FeaturedApps
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
        public App? SelectedApp
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
        public int TotalCount => _repo.GetAllApps().Count;
        public int DownloadedCount => _repo.GetAllApps().Count(a => a.IsDownloaded);

        // ── auth ─────────────────────────────────────────────────────────────
        public IAuthService AuthService => _auth;
        public ThemeService ThemeService => _theme;
        public User? CurrentUser => _auth.CurrentUser;
        public bool IsAdmin => _auth.IsAdmin;

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
        public ICommand LogoutCommand { get; }

        public event EventHandler? LogoutRequested;

        // ── constructor ──────────────────────────────────────────────────────
        public MainViewModel(
            IRepository repo,
            ILocalizationService localization,
            IAuthService auth,
            ThemeService theme
        )
        {
            _repo = repo;
            _localization = localization;
            _auth = auth;
            _theme = theme;

            DownloadCommand = new RelayCommand<App>(app =>
            {
                if (app == null)
                    return;
                if (app.IsDownloaded)
                    _repo.UninstallApp(app.Id);
                else
                    _repo.DownloadApp(app.Id);

                Reload();
                OnPropertyChanged(nameof(DownloadedCount));

                // Refresh SelectedApp reference so detail panel updates immediately
                if (_selectedApp?.Id == app.Id)
                {
                    var fresh = _repo.GetAppById(app.Id);
                    bool wasOpen = _isDetailOpen;
                    _selectedApp = null;
                    OnPropertyChanged(nameof(SelectedApp));
                    _selectedApp = fresh;
                    _isDetailOpen = wasOpen;
                    OnPropertyChanged(nameof(SelectedApp));
                }
            });

            DeleteCommand = new RelayCommand<Guid>(id =>
            {
                _repo.DeleteApp(id);
                IsDetailOpen = false;
                SelectedApp = null;
                Reload();
            });

            OpenDetailCommand = new RelayCommand<App>(app => SelectedApp = app);
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
                _repo.RestoreDefaults();
                Reload();
            });
            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));

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
            var tags = _repo.GetAllApps().SelectMany(a => a.Tags).Distinct().OrderBy(t => t);
            AllTags = new ObservableCollection<string>(tags);
        }

        private void LoadFeatured() =>
            FeaturedApps = new ObservableCollection<App>(
                _repo.GetAllApps().Where(a => a.IsFeatured)
            );

        private void RefreshList()
        {
            var result = BuildQuery();

            if (_visibleCount < int.MaxValue)
                result = result.Take(_visibleCount);

            Apps = new ObservableCollection<App>(result);
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(IsFeaturedVisible));
        }

        private IEnumerable<App> BuildQuery()
        {
            var all = _repo.GetAllApps();

            // Search takes priority
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                var q = _searchQuery.ToLowerInvariant();
                return all.Where(a =>
                    a.ShortName.ToLowerInvariant().Contains(q)
                    || a.FullName.ToLowerInvariant().Contains(q)
                    || a.Developer.ToLowerInvariant().Contains(q)
                    || a.Category.ToLowerInvariant().Contains(q)
                    || a.Tags.Any(t => t.ToLowerInvariant().Contains(q))
                );
            }

            // Tag filter
            if (_selectedTag != null)
            {
                var tagged = all.Where(a => a.Tags.Contains(_selectedTag));
                return _sortBy == "name"
                    ? tagged.OrderBy(a => a.ShortName)
                    : tagged.OrderByDescending(a => a.Rating);
            }

            // Show all categories alphabetically
            if (_selectedCategory == "Categories")
                return all.OrderBy(a => a.ShortName);

            // Resolve nav key → category string
            NavCategoryMap.TryGetValue(_selectedCategory, out var dbCat);

            // Build filtered query
            IEnumerable<App> query = all;

            if (dbCat != null)
                query = query.Where(a => a.Category == dbCat);

            if (_selectedCategory == "Installed" || _downloadedOnly)
                query = query.Where(a => a.IsDownloaded);

            if (_minPrice > 0)
                query = query.Where(a => a.FinalPrice >= _minPrice);

            if (_maxPrice < 1000)
                query = query.Where(a => a.FinalPrice <= _maxPrice);

            if (_minRating > 0)
                query = query.Where(a => a.Rating >= _minRating);

            return _sortBy switch
            {
                "name" => query.OrderBy(a => a.ShortName),
                "price" => query.OrderBy(a => a.FinalPrice),
                "rating" => query.OrderByDescending(a => a.Rating),
                "newest" => query.OrderByDescending(a => a.ReleaseDate),
                "featured" => query
                    .OrderByDescending(a => a.IsFeatured)
                    .ThenByDescending(a => a.Rating),
                _ => query.OrderByDescending(a => a.IsFeatured).ThenByDescending(a => a.Rating),
            };
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
