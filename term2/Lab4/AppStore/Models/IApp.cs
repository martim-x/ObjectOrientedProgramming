using System;
using System.Collections.Generic;

namespace Project.Models
{
    public interface IApp
    {
        Guid Id { get; set; }
        string ShortName { get; set; }
        string FullName { get; set; }
        string Description { get; set; }
        string Developer { get; set; }
        string Category { get; set; }
        double Rating { get; set; }
        int RatingCount { get; set; }
        double Price { get; set; }
        string Version { get; set; }
        double SizeMB { get; set; }
        string Country { get; set; }
        string AgeRating { get; set; }
        string Color { get; set; }
        bool IsFeatured { get; set; }
        bool IsDownloaded { get; set; }
        bool IsInStock { get; set; }
        int DownloadCount { get; set; }
        double? DiscountPercent { get; set; }
        DateTime ReleaseDate { get; set; }

        List<string> Tags { get; set; }
        List<Guid> RelatedAppIds { get; set; }

        // Computed
        double FinalPrice { get; }
        string ButtonLabel { get; }
    }
}
