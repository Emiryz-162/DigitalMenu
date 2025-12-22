using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalMenu.Data;
using DigitalMenu.ViewModels;

namespace DigitalMenu.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AccountController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: /Account/Login
        // Amacı: Admin giriş formunu gösterir
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // Zaten giriş yapmışsa dashboard'a yönlendir
            if (HttpContext.Session.GetString("IsAdmin") == "true")
            {
                return RedirectToAction("Index", "Admin");
            }

            // Settings bilgisini ViewBag'e ekle (restoran adı için)
            var settings = await _context.Settings.FirstOrDefaultAsync();
            ViewBag.Settings = settings;

            return View();
        }

        // POST: /Account/Login
        // Amacı: Admin giriş işlemini yapar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // appsettings.json'dan kullanıcı bilgilerini al
            var adminUsername = _configuration["AdminCredentials:Username"];
            var adminPassword = _configuration["AdminCredentials:Password"];

            // Kullanıcı adı ve şifre kontrolü
            if (model.Username == adminUsername && model.Password == adminPassword)
            {
                // Session'a admin bilgisi ekle
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.SetString("Username", model.Username);

                return RedirectToAction("Index", "Admin");
            }

            // Hatalı giriş
            ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
            return View(model);
        }

        // GET: /Account/Logout
        // Amacı: Admin çıkış işlemini yapar
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}