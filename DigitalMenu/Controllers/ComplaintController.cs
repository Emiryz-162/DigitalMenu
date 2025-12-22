using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalMenu.Data;
using DigitalMenu.Models;
using DigitalMenu.ViewModels;
using DigitalMenu.Helpers;

namespace DigitalMenu.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // POST: /Complaint/Submit
        // Amacı: Müşteriden gelen şikayeti kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ComplaintViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ComplaintError"] = "Lütfen tüm zorunlu alanları doldurun.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var complaint = new Complaint
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Subject = model.Subject,
                    Description = model.Description,
                    IsRead = false,
                    CreatedDate = DateTime.Now
                };

                // Görselleri yükle (varsa)
                if (model.Image1 != null)
                {
                    complaint.Image1Path = await FileUploadHelper.UploadImageAsync(
                        model.Image1, "complaints", _webHostEnvironment);
                }

                if (model.Image2 != null)
                {
                    complaint.Image2Path = await FileUploadHelper.UploadImageAsync(
                        model.Image2, "complaints", _webHostEnvironment);
                }

                if (model.Image3 != null)
                {
                    complaint.Image3Path = await FileUploadHelper.UploadImageAsync(
                        model.Image3, "complaints", _webHostEnvironment);
                }

                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();

                TempData["ComplaintSuccess"] = "Şikayet/öneriniz başarıyla gönderildi. Teşekkür ederiz!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ComplaintError"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}