using Lotomoto.Data;
using Microsoft.AspNetCore.Mvc;

namespace Lotomoto.Controllers;

public class ListingsController : Controller
{
    private readonly ListingRepository _repository;

    public ListingsController(ListingRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string? query, string? category, int? minYear, int? maxYear, decimal? minPrice, decimal? maxPrice)
    {
        var listings = _repository.Search(query, category, minYear, maxYear, minPrice, maxPrice);
        ViewData["Categories"] = _repository.GetCategories();
        ViewData["SelectedCategory"] = category;
        ViewData["Query"] = query;
        ViewData["MinYear"] = minYear;
        ViewData["MaxYear"] = maxYear;
        ViewData["MinPrice"] = minPrice;
        ViewData["MaxPrice"] = maxPrice;
        return View(listings);
    }

    public IActionResult Details(int id)
    {
        var listing = _repository.Get(id);
        if (listing == null)
        {
            return NotFound();
        }

        return View(listing);
    }
}
