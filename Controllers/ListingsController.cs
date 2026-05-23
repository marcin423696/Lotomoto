using Microsoft.AspNetCore.Mvc;
using Lotomoto.Data;
using System.Linq;

namespace Lotomoto.Controllers
{
    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? query, string? category, int? minYear, int? maxYear, decimal? minPrice, decimal? maxPrice)
        {
            var listings = _context.CarListings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                listings = listings.Where(x => x.Title.Contains(query) || x.Description.Contains(query));
            }
            if (!string.IsNullOrWhiteSpace(category))
            {
                listings = listings.Where(x => x.Category == category);
            }
            if (minYear.HasValue) listings = listings.Where(x => x.Year >= minYear.Value);
            if (maxYear.HasValue) listings = listings.Where(x => x.Year <= maxYear.Value);
            if (minPrice.HasValue) listings = listings.Where(x => x.Price >= minPrice.Value);
            if (maxPrice.HasValue) listings = listings.Where(x => x.Price <= maxPrice.Value);

            ViewData["Categories"] = _context.CarListings.Select(x => x.Category).Distinct().ToList();
            ViewData["SelectedCategory"] = category;
            ViewData["Query"] = query;
            ViewData["MinYear"] = minYear;
            ViewData["MaxYear"] = maxYear;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

            return View(listings.ToList());
        }

        public IActionResult Details(int id)
        {
            var listing = _context.CarListings.FirstOrDefault(x => x.Id == id);
            if (listing == null) return NotFound();

            return View(listing);
        }
    }
}