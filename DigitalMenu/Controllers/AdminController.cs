using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using DigitalMenu.Data;
using DigitalMenu.Models;
using DigitalMenu.ViewModels;
using DigitalMenu.Helpers;

namespace DigitalMenu.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Her action'dan önce çalışır - Admin kontrolü yapar
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                context.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(context);
        }

        // GET: /Admin/Index
        // Amacı: Admin ana sayfası - İstatistikler ve hızlı erişim
        public async Task<IActionResult> Index()
        {
            ViewBag.CategoryCount = await _context.Categories.CountAsync();
            ViewBag.ProductCount = await _context.Products.CountAsync();
            ViewBag.ComplaintCount = await _context.Complaints.CountAsync();
            ViewBag.VisitCount = await _context.SiteVisits.CountAsync();
            ViewBag.TodayVisitCount = await _context.SiteVisits
                .Where(v => v.VisitDate.Date == DateTime.Today)
                .CountAsync();
            ViewBag.Settings = await _context.Settings.FirstOrDefaultAsync();

            return View();
        }

        #region Kategori İşlemleri

        // GET: /Admin/Categories
        // Amacı: Tüm kategorileri listeler
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)  // Bu satırı ekle
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return View(categories);
        }

        // GET: /Admin/CreateCategory
        // Amacı: Kategori ekleme formunu gösterir
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: /Admin/CreateCategory
        // Amacı: Yeni kategori oluşturur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Görsel yükleme
                if (model.ImageFile != null)
                {
                    model.ImagePath = await FileUploadHelper.UploadImageAsync(
                        model.ImageFile, "categories", _webHostEnvironment);
                }

                // En büyük SortOrder değerini bul ve 1 ekle
                var maxSortOrder = await _context.Categories.AnyAsync()
                    ? await _context.Categories.MaxAsync(c => c.SortOrder)
                    : 0;

                // Category modeline dönüştür
                var category = new Category
                {
                    Name = model.Name,
                    ImagePath = model.ImagePath,
                    SortOrder = maxSortOrder + 1,
                    CreatedDate = DateTime.Now
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kategori başarıyla eklendi";
                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
                return View(model);
            }
        }

        // GET: /Admin/EditCategory/5
        // Amacı: Kategori düzenleme formunu gösterir
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ImagePath = category.ImagePath,
                SortOrder = category.SortOrder
            };

            return View(model);
        }

        // POST: /Admin/EditCategory/5
        // Amacı: Kategoriyi günceller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var category = await _context.Categories.FindAsync(model.Id);

                if (category == null)
                {
                    return NotFound();
                }

                // Yeni görsel yüklendiyse
                if (model.ImageFile != null)
                {
                    // Eski görseli sil
                    FileUploadHelper.DeleteImage(category.ImagePath, _webHostEnvironment);

                    // Yeni görseli yükle
                    category.ImagePath = await FileUploadHelper.UploadImageAsync(
                        model.ImageFile, "categories", _webHostEnvironment);
                }

                category.Name = model.Name;

                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi";
                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
                return View(model);
            }
        }

        // POST: /Admin/DeleteCategory/5
        // Amacı: Kategoriyi siler (ürünleri de cascade ile silinir)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                // Görseli sil
                FileUploadHelper.DeleteImage(category.ImagePath, _webHostEnvironment);

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kategori başarıyla silindi";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction(nameof(Categories));
        }

        #endregion

        #region Ürün İşlemleri

        // GET: /Admin/Products/5
        // Amacı: Belirtilen kategorideki ürünleri listeler
        public async Task<IActionResult> Products(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Products.OrderBy(p => p.SortOrder))
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            ViewBag.Category = category;
            return View(category.Products);
        }

        // GET: /Admin/CreateProduct?categoryId=5
        // Amacı: Ürün ekleme formunu gösterir
        [HttpGet]
        public async Task<IActionResult> CreateProduct(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = category.Name;

            var model = new ProductViewModel
            {
                CategoryId = categoryId
            };

            return View(model);
        }

        // POST: /Admin/CreateProduct
        // Amacı: Yeni ürün oluşturur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(model.CategoryId);
                ViewBag.CategoryName = category?.Name;
                return View(model);
            }

            try
            {
                // Görsel yükleme
                if (model.ImageFile != null)
                {
                    model.ImagePath = await FileUploadHelper.UploadImageAsync(
                        model.ImageFile, "products", _webHostEnvironment);
                }

                // En büyük SortOrder değerini bul (kategorideki ürünler için)
                var maxSortOrder = await _context.Products
                    .Where(p => p.CategoryId == model.CategoryId)
                    .AnyAsync()
                    ? await _context.Products
                        .Where(p => p.CategoryId == model.CategoryId)
                        .MaxAsync(p => p.SortOrder)
                    : 0;

                // Product modeline dönüştür
                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    Description = model.Description,
                    Calories = model.Calories,
                    ImagePath = model.ImagePath,
                    CategoryId = model.CategoryId,
                    SortOrder = maxSortOrder + 1,
                    CreatedDate = DateTime.Now
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ürün başarıyla eklendi";
                return RedirectToAction(nameof(Products), new { categoryId = model.CategoryId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
                var category = await _context.Categories.FindAsync(model.CategoryId);
                ViewBag.CategoryName = category?.Name;
                return View(model);
            }
        }

        // GET: /Admin/EditProduct/5
        // Amacı: Ürün düzenleme formunu gösterir
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = product.Category.Name;

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Calories = product.Calories,
                ImagePath = product.ImagePath,
                CategoryId = product.CategoryId,
                SortOrder = product.SortOrder
            };

            return View(model);
        }

        // POST: /Admin/EditProduct/5
        // Amacı: Ürünü günceller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cat = await _context.Categories.FindAsync(model.CategoryId);
                ViewBag.CategoryName = cat?.Name;
                return View(model);
            }

            try
            {
                var product = await _context.Products.FindAsync(model.Id);

                if (product == null)
                {
                    return NotFound();
                }

                // Yeni görsel yüklendiyse
                if (model.ImageFile != null)
                {
                    // Eski görseli sil
                    FileUploadHelper.DeleteImage(product.ImagePath, _webHostEnvironment);

                    // Yeni görseli yükle
                    product.ImagePath = await FileUploadHelper.UploadImageAsync(
                        model.ImageFile, "products", _webHostEnvironment);
                }

                product.Name = model.Name;
                product.Price = model.Price;
                product.Description = model.Description;
                product.Calories = model.Calories;

                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi";
                return RedirectToAction(nameof(Products), new { categoryId = product.CategoryId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
                var cat = await _context.Categories.FindAsync(model.CategoryId);
                ViewBag.CategoryName = cat?.Name;
                return View(model);
            }
        }

        // POST: /Admin/DeleteProduct/5
        // Amacı: Ürünü siler
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                var categoryId = product.CategoryId;

                // Görseli sil
                FileUploadHelper.DeleteImage(product.ImagePath, _webHostEnvironment);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ürün başarıyla silindi";
                return RedirectToAction(nameof(Products), new { categoryId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Ayarlar İşlemleri

        // GET: /Admin/Settings
        // Amacı: Genel ayarları gösterir
        public async Task<IActionResult> Settings()
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();

            if (settings == null)
            {
                return NotFound();
            }

            var model = new SettingsViewModel
            {
                Id = settings.Id,
                RestaurantName = settings.RestaurantName,
                WelcomeText = settings.WelcomeText,
                LogoPath = settings.LogoPath
            };

            return View(model);
        }
        // POST: /Admin/Settings
        // Amacı: Genel ayarları günceller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var settings = await _context.Settings.FindAsync(model.Id);

                if (settings == null)
                {
                    return NotFound();
                }

                // Logo yükleme
                if (model.LogoFile != null)
                {
                    // Eski logoyu sil
                    if (!string.IsNullOrEmpty(settings.LogoPath))
                    {
                        FileUploadHelper.DeleteImage(settings.LogoPath, _webHostEnvironment);
                    }

                    // Yeni logoyu yükle
                    settings.LogoPath = await FileUploadHelper.UploadImageAsync(
                        model.LogoFile, "settings", _webHostEnvironment);
                }

                settings.RestaurantName = model.RestaurantName;
                settings.WelcomeText = model.WelcomeText;
                settings.UpdatedDate = DateTime.Now;

                _context.Update(settings);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ayarlar başarıyla güncellendi";
                return RedirectToAction(nameof(Settings));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
                return View(model);
            }
        }

        #endregion

        #region Sıralama İşlemleri

        // POST: /Admin/UpdateCategoryOrder
        // Amacı: Kategori sıralamasını günceller (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateCategoryOrder([FromBody] List<int> categoryIds)
        {
            try
            {
                for (int i = 0; i < categoryIds.Count; i++)
                {
                    var category = await _context.Categories.FindAsync(categoryIds[i]);
                    if (category != null)
                    {
                        category.SortOrder = i + 1;
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sıralama başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        // POST: /Admin/UpdateProductOrder
        // Amacı: Ürün sıralamasını günceller (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateProductOrder([FromBody] List<int> productIds)
        {
            try
            {
                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = await _context.Products.FindAsync(productIds[i]);
                    if (product != null)
                    {
                        product.SortOrder = i + 1;
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sıralama başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        #endregion

        #region Şikayet İşlemleri

        // GET: /Admin/Complaints
        // Amacı: Tüm şikayetleri listeler
        public async Task<IActionResult> Complaints()
        {
            var complaints = await _context.Complaints
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(complaints);
        }

        // GET: /Admin/ComplaintDetail/5
        // Amacı: Şikayet detayını gösterir ve okundu işaretler
        public async Task<IActionResult> ComplaintDetail(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);

            if (complaint == null)
            {
                return NotFound();
            }

            // Okundu olarak işaretle
            if (!complaint.IsRead)
            {
                complaint.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(complaint);
        }

        // POST: /Admin/DeleteComplaint/5
        // Amacı: Şikayeti siler
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            try
            {
                var complaint = await _context.Complaints.FindAsync(id);

                if (complaint == null)
                {
                    return NotFound();
                }

                // Görselleri sil
                if (!string.IsNullOrEmpty(complaint.Image1Path))
                {
                    FileUploadHelper.DeleteImage(complaint.Image1Path, _webHostEnvironment);
                }

                if (!string.IsNullOrEmpty(complaint.Image2Path))
                {
                    FileUploadHelper.DeleteImage(complaint.Image2Path, _webHostEnvironment);
                }

                if (!string.IsNullOrEmpty(complaint.Image3Path))
                {
                    FileUploadHelper.DeleteImage(complaint.Image3Path, _webHostEnvironment);
                }

                _context.Complaints.Remove(complaint);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Şikayet başarıyla silindi";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction(nameof(Complaints));
        }

        #endregion
    }
}