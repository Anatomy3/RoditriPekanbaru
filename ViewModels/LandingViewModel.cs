using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.ViewModels
{
    public class LandingViewModel
    {
        public List<string> Brands { get; set; } = new List<string>();
        public List<Mobil> Cars { get; set; } = new List<Mobil>();

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCars { get; set; } = 0;

        // Filters
        public string? SelectedMerek { get; set; }
        public string? SearchTerm { get; set; }
        public decimal? MinHarga { get; set; }
        public decimal? MaxHarga { get; set; }
        public int? TahunMin { get; set; }
        public int? TahunMax { get; set; }

        // Helper properties
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;

        // Featured brands with logos (you can expand this)
        public List<BrandInfo> FeaturedBrands => new List<BrandInfo>
        {
            new BrandInfo { Name = "Toyota", LogoUrl = "/images/brands/toyota.png" },
            new BrandInfo { Name = "Honda", LogoUrl = "/images/brands/honda.png" },
            new BrandInfo { Name = "Suzuki", LogoUrl = "/images/brands/suzuki.png" },
            new BrandInfo { Name = "Mitsubishi", LogoUrl = "/images/brands/mitsubishi.png" },
            new BrandInfo { Name = "Daihatsu", LogoUrl = "/images/brands/daihatsu.png" },
            new BrandInfo { Name = "Nissan", LogoUrl = "/images/brands/nissan.png" },
            new BrandInfo { Name = "BMW", LogoUrl = "/images/brands/bmw.png" },
            new BrandInfo { Name = "Mercedes-Benz", LogoUrl = "/images/brands/mercedes.png" },
            new BrandInfo { Name = "Hyundai", LogoUrl = "/images/brands/hyundai.png" },
            new BrandInfo { Name = "KIA", LogoUrl = "/images/brands/kia.png" },
            new BrandInfo { Name = "Mazda", LogoUrl = "/images/brands/mazda.png" },
            new BrandInfo { Name = "Volkswagen", LogoUrl = "/images/brands/volkswagen.png" }
        };
    }

    public class BrandInfo
    {
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
    }
}