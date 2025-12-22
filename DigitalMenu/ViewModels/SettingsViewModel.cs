using System.ComponentModel.DataAnnotations;

namespace DigitalMenu.ViewModels
{
    public class SettingsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Restoran adı zorunludur")]
        [Display(Name = "Restoran Adı")]
        public string RestaurantName { get; set; }

        [Display(Name = "Hoş Geldiniz Mesajı")]
        public string? WelcomeText { get; set; }

        [Display(Name = "Mevcut Logo")]
        public string? LogoPath { get; set; }

        [Display(Name = "Yeni Logo")]
        public IFormFile? LogoFile { get; set; }
    }
}