using Lotomoto.Data;
using Lotomoto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lotomoto.Controllers;

public class AdminController : Controller
{
    private readonly ListingRepository _repository;
    private readonly IWebHostEnvironment _env;

    // Zarządzanie użytkownikami i sesjami przez bazodanowe ASP.NET Core Identity
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AdminController(
        ListingRepository repository,
        IWebHostEnvironment env,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _repository = repository;
        _env = env;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        return View(new AdminLoginModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Identity automatycznie haszuje wpisane hasło i porównuje je z bazą danych SQL
        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction(nameof(Dashboard));
        }

        ModelState.AddModelError(string.Empty, "Nieprawidłowy e-mail lub hasło.");
        return View(model);
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        var listings = _repository.GetAll();
        return View(listings);
    }

    [Authorize]
    public IActionResult Create()
    {
        return View(new CarListing());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarListing model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            model.ImageUrl = await SaveImageAsync(imageFile);
        }

        if (string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            model.ImageUrl = "https://via.placeholder.com/900x520?text=No+Image";
        }

        _repository.Add(model);
        return RedirectToAction(nameof(Dashboard));
    }

    [Authorize]
    public IActionResult Edit(int id)
    {
        var listing = _repository.Get(id);
        if (listing == null)
        {
            return NotFound();
        }

        return View(listing);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CarListing model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = _repository.Get(id);
        if (existing == null)
        {
            return NotFound();
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            model.ImageUrl = await SaveImageAsync(imageFile);
        }
        else
        {
            model.ImageUrl = existing.ImageUrl;
        }

        model.Id = id;
        _repository.Update(model);
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _repository.Delete(id);
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await imageFile.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }
}