using System.ComponentModel.DataAnnotations;

namespace DigitalMenu.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }

        [Display(Name = "Mevcut Görsel")]
        public string? ImagePath { get; set; }

        [Display(Name = "Yeni Görsel")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Sıra")]
        public int SortOrder { get; set; }
    }
}