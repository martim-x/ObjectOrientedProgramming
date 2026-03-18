using System;
using System.Collections.Generic;

namespace AppStore.Models
{
    /// <summary>
    /// Core domain model. Serialised to JSON by Newtonsoft.Json.
    /// </summary>
    public class AppItem
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
        public string Version { get; set; } = "1.0";
        public double SizeMB { get; set; }
        public string Country { get; set; } = string.Empty;
        public string AgeRating { get; set; } = "4+";
        public string Color { get; set; } = "#007AFF";
        public bool IsFeatured { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsInStock { get; set; } = true;
        public int DownloadCount { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        public List<string> Tags { get; set; } = new();
        public List<Guid> RelatedAppIds { get; set; } = new();

        // Computed — not serialised
        public bool IsFree => Price <= 0;
        public double FinalPrice =>
            DiscountPercent.HasValue ? Price * (1 - DiscountPercent.Value / 100.0) : Price;
    }
}
