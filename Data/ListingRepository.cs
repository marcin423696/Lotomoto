using System.Text.Json;
using Lotomoto.Models;

namespace Lotomoto.Data;

public class ListingRepository
{
    private readonly string _dataFile;
    private readonly object _lock = new();
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
    private List<CarListing> _items = new();

    public ListingRepository(IHostEnvironment hostEnvironment)
    {
        _dataFile = Path.Combine(hostEnvironment.ContentRootPath, "Data", "carlistings.json");
        EnsureData();
    }

    private void EnsureData()
    {
        lock (_lock)
        {
            var directory = Path.GetDirectoryName(_dataFile);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_dataFile))
            {
                _items = GetSeedData();
                Save();
                return;
            }

            var json = File.ReadAllText(_dataFile);
            if (string.IsNullOrWhiteSpace(json))
            {
                _items = GetSeedData();
                Save();
                return;
            }

            try
            {
                _items = JsonSerializer.Deserialize<List<CarListing>>(json, _serializerOptions) ?? GetSeedData();
            }
            catch
            {
                _items = GetSeedData();
                Save();
            }
        }
    }

    private static List<CarListing> GetSeedData() => new()
    {
        new CarListing
        {
            Id = 1,
            Title = "Ford Mustang GT",
            Price = 129900,
            Mileage = 52000,
            Year = 2018,
            Category = "Coupe",
            Version = "GT 5.0 V8",
            Description = "Sportowe coupe z mocnym silnikiem V8, skórzaną tapicerką i dynamicznym wyglądem.",
            ImageUrl = "https://via.placeholder.com/900x520?text=Ford+Mustang+GT"
        },
        new CarListing
        {
            Id = 2,
            Title = "Toyota Corolla Hybrid",
            Price = 75900,
            Mileage = 24000,
            Year = 2021,
            Category = "Hatchback",
            Version = "1.8 Hybrid Comfort",
            Description = "Ekonomiczny kompakt ze sprawdzonym układem hybrydowym, idealny do miasta.",
            ImageUrl = "https://via.placeholder.com/900x520?text=Toyota+Corolla+Hybrid"
        },
        new CarListing
        {
            Id = 3,
            Title = "BMW X3 xDrive",
            Price = 154900,
            Mileage = 68000,
            Year = 2019,
            Category = "SUV",
            Version = "xDrive30i",
            Description = "Luksusowy SUV z napędem na cztery koła, przestronnym wnętrzem i zaawansowanymi systemami bezpieczeństwa.",
            ImageUrl = "https://via.placeholder.com/900x520?text=BMW+X3"
        }
    };

    private void Save()
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(_items, _serializerOptions);
            File.WriteAllText(_dataFile, json);
        }
    }

    public IEnumerable<CarListing> GetAll() => _items.OrderByDescending(x => x.Year).ThenBy(x => x.Price).ToArray();

    public CarListing? Get(int id) => _items.FirstOrDefault(x => x.Id == id);

    public IEnumerable<string> GetCategories() => _items.Select(x => x.Category).Distinct().OrderBy(x => x);

    public void Add(CarListing listing)
    {
        lock (_lock)
        {
            listing.Id = _items.Any() ? _items.Max(x => x.Id) + 1 : 1;
            _items.Add(listing);
            Save();
        }
    }

    public void Update(CarListing listing)
    {
        lock (_lock)
        {
            var existing = _items.FirstOrDefault(x => x.Id == listing.Id);
            if (existing is null)
            {
                return;
            }

            existing.Title = listing.Title;
            existing.Price = listing.Price;
            existing.Mileage = listing.Mileage;
            existing.Year = listing.Year;
            existing.Category = listing.Category;
            existing.Version = listing.Version;
            existing.Description = listing.Description;
            existing.ImageUrl = listing.ImageUrl;
            Save();
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            var existing = _items.FirstOrDefault(x => x.Id == id);
            if (existing != null)
            {
                _items.Remove(existing);
                Save();
            }
        }
    }

    public IEnumerable<CarListing> Search(string? query, string? category, int? minYear, int? maxYear, decimal? minPrice, decimal? maxPrice)
    {
        var results = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim().ToLowerInvariant();
            results = results.Where(x => x.Title.ToLowerInvariant().Contains(normalized)
                                         || x.Description.ToLowerInvariant().Contains(normalized)
                                         || x.Category.ToLowerInvariant().Contains(normalized)
                                         || x.Version.ToLowerInvariant().Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            results = results.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (minYear.HasValue)
        {
            results = results.Where(x => x.Year >= minYear.Value);
        }

        if (maxYear.HasValue)
        {
            results = results.Where(x => x.Year <= maxYear.Value);
        }

        if (minPrice.HasValue)
        {
            results = results.Where(x => x.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            results = results.Where(x => x.Price <= maxPrice.Value);
        }

        return results.OrderByDescending(x => x.Year).ThenBy(x => x.Price).ToArray();
    }
}
