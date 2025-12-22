using System.ComponentModel.DataAnnotations;

namespace DigitalMenu.ViewModels
{
    public class ComplaintViewModel
    {
        [Required(ErrorMessage = "İsim Soyisim zorunludur")]
        [Display(Name = "İsim Soyisim")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin")]
        [Display(Name = "Telefon (Opsiyonel)")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        [Display(Name = "Şikayet/Öneri Başlığı")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Görsel 1 (Opsiyonel)")]
        public IFormFile? Image1 { get; set; }

        [Display(Name = "Görsel 2 (Opsiyonel)")]
        public IFormFile? Image2 { get; set; }

        [Display(Name = "Görsel 3 (Opsiyonel)")]
        public IFormFile? Image3 { get; set; }
    }
}