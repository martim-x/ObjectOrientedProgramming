using System;
using System.Collections.Generic;
using System.Linq;
using AppStore.Data;
using AppStore.Models;

namespace AppStore.Services
{
    public interface IAppService
    {
        IEnumerable<AppItem> GetAll();
        IEnumerable<AppItem> GetFeatured();
        IEnumerable<AppItem> Search(string query);
        IEnumerable<AppItem> Filter(
            string? category,
            double? minPrice,
            double? maxPrice,
            double? minRating,
            bool? downloadedOnly,
            string? sortBy
        );
        AppItem? GetById(Guid id);
        void Add(AppItem app);
        void Update(AppItem app);
        void Delete(Guid id);
        void Download(Guid id);
        void Uninstall(Guid id);
        void RestoreDefaults();
        int GetTotalCount();
        int GetDownloadedCount();
    }

    public class AppService : IAppService
    {
        private readonly IAppRepository _repo;

        public AppService(IAppRepository repo) => _repo = repo;

        public IEnumerable<AppItem> GetAll() => _repo.GetAll();

        public IEnumerable<AppItem> GetFeatured() =>
            _repo.GetAll().Where(a => a.IsFeatured).Take(5);

        public AppItem? GetById(Guid id) => _repo.GetById(id);

        public IEnumerable<AppItem> Search(string q) => _repo.Search(q);

        public IEnumerable<AppItem> Filter(
            string? cat,
            double? minP,
            double? maxP,
            double? minR,
            bool? dlOnly,
            string? sort
        ) => _repo.Filter(cat, minP, maxP, minR, dlOnly, sort);

        public void Add(AppItem app) => _repo.Add(app);

        public void Update(AppItem app) => _repo.Update(app);

        public void Delete(Guid id) => _repo.Delete(id);

        public void Download(Guid id) => _repo.Download(id);

        public void Uninstall(Guid id) => _repo.Uninstall(id);

        public void RestoreDefaults() => _repo.RestoreDefaults();

        public int GetTotalCount() => _repo.GetAll().Count();

        public int GetDownloadedCount() => _repo.GetAll().Count(a => a.IsDownloaded);
    }
}
