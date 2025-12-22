using System.ComponentModel.DataAnnotations;

namespace DigitalMenu.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Display(Name = "Fiyat")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [Display(Name = "Açıklama")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string Description { get; set; }

        [Display(Name = "Kalori (kcal) - Opsiyonel")]
        [Range(0, 10000, ErrorMessage = "Kalori değeri 0-10000 arasında olmalıdır")]
        public int? Calories { get; set; }

        [Display(Name = "Mevcut Görsel")]
        public string? ImagePath { get; set; }

        [Display(Name = "Yeni Görsel")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Display(Name = "Sıra")]
        public int SortOrder { get; set; }
    }
}