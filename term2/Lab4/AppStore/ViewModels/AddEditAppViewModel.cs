using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AppStore.Commands;
using AppStore.Models;
using AppStore.Services;

namespace AppStore.ViewModels
{
    public class AddEditAppViewModel : ViewModelBase
    {
        private readonly IAppService _service;
        private AppItem _app;
        private bool _isEditMode;

        public AppItem App
        {
            get => _app;
            set => SetField(ref _app, value);
        }
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetField(ref _isEditMode, value);
        }

        public ObservableCollection<string> AvailableCategories { get; } =
            new()
            {
                "Development",
                "Design",
                "Productivity",
                "Games",
                "Media",
                "Social",
                "Utilities",
                "Education",
                "Finance",
                "Health",
            };

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public Action? OnSaved { get; set; }
        public Action? OnCancelled { get; set; }

        public AddEditAppViewModel(IAppService service, AppItem? existing = null)
        {
            _service = service;

            if (existing != null)
            {
                _app = Clone(existing);
                _isEditMode = true;
            }
            else
            {
                _app = new AppItem { ReleaseDate = DateTime.Now };
                _isEditMode = false;
            }

            SaveCommand = new RelayCommand(Save, () => !string.IsNullOrWhiteSpace(App.ShortName));
            CancelCommand = new RelayCommand(() => OnCancelled?.Invoke());
        }

        private void Save()
        {
            if (_isEditMode)
                _service.Update(App);
            else
                _service.Add(App);
            OnSaved?.Invoke();
        }

        private static AppItem Clone(AppItem s) =>
            new()
            {
                Id = s.Id,
                ShortName = s.ShortName,
                FullName = s.FullName,
                Description = s.Description,
                Developer = s.Developer,
                Category = s.Category,
                Rating = s.Rating,
                RatingCount = s.RatingCount,
                Price = s.Price,
                Version = s.Version,
                SizeMB = s.SizeMB,
                Country = s.Country,
                AgeRating = s.AgeRating,
                Color = s.Color,
                IsFeatured = s.IsFeatured,
                IsDownloaded = s.IsDownloaded,
                IsInStock = s.IsInStock,
                DownloadCount = s.DownloadCount,
                DiscountPercent = s.DiscountPercent,
                ReleaseDate = s.ReleaseDate,
                Tags = new(s.Tags),
                RelatedAppIds = new(s.RelatedAppIds),
            };
    }
}
