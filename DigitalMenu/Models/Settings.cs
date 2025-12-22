using System.ComponentModel.DataAnnotations;

namespace DigitalMenu.Models
{
    public class Settings
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Restoran adı zorunludur")]
        [StringLength(200, ErrorMessage = "Restoran adı en fazla 200 karakter olabilir")]
        public string RestaurantName { get; set; }

        [StringLength(500)]
        public string? WelcomeText { get; set; }

        // Logo
        public string? LogoPath { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}