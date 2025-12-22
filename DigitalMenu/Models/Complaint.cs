using System.ComponentModel.DataAnnotations;

namespace DigitalMenu.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim Soyisim zorunludur")]
        [StringLength(200, ErrorMessage = "İsim Soyisim en fazla 200 karakter olabilir")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin")]
        [StringLength(200)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(300, ErrorMessage = "Başlık en fazla 300 karakter olabilir")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir")]
        public string Description { get; set; }

        // Görseller (3'e kadar, opsiyonel)
        public string? Image1Path { get; set; }
        public string? Image2Path { get; set; }
        public string? Image3Path { get; set; }

        // Durum takibi
        public bool IsRead { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}