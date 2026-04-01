using System;
using System.Collections.Generic;

namespace Project.Models
{
    public class App : IApp
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public double Price { get; set; }
        public string Version { get; set; } = string.Empty;
        public double SizeMB { get; set; }
        public string Country { get; set; } = string.Empty;
        public string AgeRating { get; set; } = "4+";
        public string Color { get; set; } = "#007AFF";
        public bool IsFeatured { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsInStock { get; set; } = true;
        public int DownloadCount { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime ReleaseDate { get; set; }

        public List<string> Tags { get; set; } = new();
        public List<Guid> RelatedAppIds { get; set; } = new();

        // Computed: apply discount if set
        public double FinalPrice =>
            DiscountPercent.HasValue ? Price * (1.0 - DiscountPercent.Value / 100.0) : Price;
    }
}
