using System;
using System.Collections.Generic;
using System.Linq;
using AppStore.Models;

namespace AppStore.Data
{
    public interface IAppRepository
    {
        IEnumerable<AppItem> GetAll();
        AppItem? GetById(Guid id);
        IEnumerable<AppItem> Search(string query);
        IEnumerable<AppItem> Filter(
            string? category,
            double? minPrice,
            double? maxPrice,
            double? minRating,
            bool? downloadedOnly,
            string? sortBy
        );
        void Add(AppItem app);
        void Update(AppItem app);
        void Delete(Guid id);
        void Download(Guid id);
        void Uninstall(Guid id);
        void RestoreDefaults();
    }

    public class AppRepository : IAppRepository
    {
        private readonly IStorageService _storage;
        private List<AppItem> _cache;

        public AppRepository(IStorageService storage)
        {
            _storage = storage;
            _cache = storage.LoadApps();
        }

        public IEnumerable<AppItem> GetAll() => _cache.ToList();

        public AppItem? GetById(Guid id) => _cache.FirstOrDefault(a => a.Id == id);

        public IEnumerable<AppItem> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _cache;
            var q = query.ToLower();
            return _cache.Where(a =>
                a.ShortName.ToLower().Contains(q)
                || a.FullName.ToLower().Contains(q)
                || a.Description.ToLower().Contains(q)
                || a.Developer.ToLower().Contains(q)
                || a.Tags.Any(t => t.ToLower().Contains(q))
            );
        }

        public IEnumerable<AppItem> Filter(
            string? category,
            double? minPrice,
            double? maxPrice,
            double? minRating,
            bool? downloadedOnly,
            string? sortBy
        )
        {
            var q = _cache.AsEnumerable();
            if (!string.IsNullOrEmpty(category))
                q = q.Where(a => a.Category == category);
            if (minPrice.HasValue)
                q = q.Where(a => a.FinalPrice >= minPrice.Value);
            if (maxPrice.HasValue)
                q = q.Where(a => a.FinalPrice <= maxPrice.Value);
            if (minRating.HasValue)
                q = q.Where(a => a.Rating >= minRating.Value);
            if (downloadedOnly == true)
                q = q.Where(a => a.IsDownloaded);
            return (
                sortBy switch
                {
                    "rating" => q.OrderByDescending(a => a.Rating),
                    "price_asc" => q.OrderBy(a => a.FinalPrice),
                    "price_desc" => q.OrderByDescending(a => a.FinalPrice),
                    "downloads" => q.OrderByDescending(a => a.DownloadCount),
                    "name" => q.OrderBy(a => a.ShortName),
                    "newest" => q.OrderByDescending(a => a.ReleaseDate),
                    _ => q.OrderByDescending(a => a.IsFeatured),
                }
            ).ToList();
        }

        public void Add(AppItem app)
        {
            _storage.AddApp(app);
            Reload();
        }

        public void Update(AppItem app)
        {
            _storage.UpdateApp(app);
            Reload();
        }

        public void Delete(Guid id)
        {
            _storage.DeleteApp(id);
            Reload();
        }

        public void Download(Guid id)
        {
            _storage.DownloadApp(id);
            Reload();
        }

        public void Uninstall(Guid id)
        {
            _storage.UninstallApp(id);
            Reload();
        }

        public void RestoreDefaults()
        {
            _storage.RestoreDefaults();
            Reload();
        }

        private void Reload() => _cache = _storage.LoadApps();
    }
}
