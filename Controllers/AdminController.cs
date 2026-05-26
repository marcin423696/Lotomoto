using System.Security.Claims;
using Lotomoto.Data;
using Lotomoto.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Lotomoto.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const string AdminEmail = "admin@lotomoto.local";
        private const string AdminPassword = "Admin123!";

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
            if (!ModelState.IsValid) return View(model);

            if (!string.Equals(model.Email, AdminEmail, StringComparison.OrdinalIgnoreCase) || model.Password != AdminPassword)
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy e-mail lub hasło.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Administrator"),
                new Claim(ClaimTypes.Email, AdminEmail)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction(nameof(Dashboard));
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var listings = _context.CarListings.ToList();
            return View(listings);
        }

        // Pozwalamy anonimowym użytkownikom dodawać ogłoszenia
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View(new CarListing());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarListing model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            if (imageFile != null && imageFile.Length > 0)
            {
                model.ImageUrl = await SaveImageAsync(imageFile);
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                model.ImageUrl = "https://via.placeholder.com/900x520?text=No+Image";
            }

            _context.CarListings.Add(model);
            _context.SaveChanges();

            
            return RedirectToAction("Index", "Listings");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var listing = _context.CarListings.FirstOrDefault(x => x.Id == id);
            if (listing == null) return NotFound();
            return View(listing);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarListing model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = _context.CarListings.FirstOrDefault(x => x.Id == id);
            if (existing == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                existing.ImageUrl = await SaveImageAsync(imageFile);
            }

            existing.Title = model.Title;
            existing.Price = model.Price;
            existing.Mileage = model.Mileage;
            existing.Year = model.Year;
            existing.Category = model.Category;
            existing.Version = model.Version;
            existing.Description = model.Description;

            _context.CarListings.Update(existing);
            _context.SaveChanges();
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var listing = _context.CarListings.FirstOrDefault(x => x.Id == id);
            if (listing != null)
            {
                _context.CarListings.Remove(listing);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await imageFile.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }
    }
}