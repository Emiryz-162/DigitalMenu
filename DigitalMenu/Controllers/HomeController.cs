using DigitalMenu.Data;
using DigitalMenu.Models;
using DigitalMenu.ViewModels;
using DigitalMenu.ViewModels.DigitalMenu.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index veya /
        // Amacı: Ana sayfayı gösterir
        public async Task<IActionResult> Index()
        {
            // Ziyaret kaydı oluştur
            try
            {
                var visit = new SiteVisit
                {
                    VisitDate = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                };

                _context.SiteVisits.Add(visit);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Hata olursa sessizce devam et (ziyaret kaydı kritik değil)
            }

            var settings = await _context.Settings.FirstOrDefaultAsync();
            var categories = await _context.Categories
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            ViewBag.Settings = settings;
            ViewBag.Categories = categories;

            return View();
        }

        // GET: /Home/Category/5
        // Amacı: Seçilen kategorinin ürünlerini gösterir
        // GET: /Home/Category/5
        public async Task<IActionResult> Category(int id)
        {
            // Kategoriyi kontrol et
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == id);
            if (!categoryExists)
            {
                return NotFound();
            }

            // ViewModel oluştur
            var viewModel = new ViewModels.CategoryPageViewModel
            {
                CategoryId = id,
                CategoryName = await _context.Categories
                    .Where(c => c.Id == id)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync(),

                Products = await _context.Products
                    .Where(p => p.CategoryId == id)
                    .OrderBy(p => p.SortOrder)
                    .Select(p => new ViewModels.ProductDisplayModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description,
                        Calories = p.Calories,
                        ImagePath = p.ImagePath,
                        SortOrder = p.SortOrder
                    })
                    .ToListAsync(),

                AllCategories = await _context.Categories
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new CategoryNavModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ImagePath = c.ImagePath,  // BUNU EKLE
                        SortOrder = c.SortOrder
                    })
                    .ToListAsync()
            };

            // Settings bilgisini ViewBag'e ekle (artık kullanmıyoruz ama bırakalım)
            var settings = await _context.Settings.FirstOrDefaultAsync();
            ViewBag.Settings = settings;

            return View(viewModel);
        }
    }
}