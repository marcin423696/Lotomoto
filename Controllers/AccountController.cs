using System.Security.Claims;
using Lotomoto.Data;
using Lotomoto.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Lotomoto.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Ten e-mail jest ju¿ zajêty.");
                    return View(model);
                }

                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            
            if (email == "admin@lotomoto.local" && password == "Admin123!")
            {
                await ZalogujUzytkownika(email, "Administrator");
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (user != null)
            {
                await ZalogujUzytkownika(user.Email, "Uzytkownik");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nieprawid³owy e-mail lub has³o.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        
        private async Task ZalogujUzytkownika(string email, string rola)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, rola)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}