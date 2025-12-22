using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DigitalMenu.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kategori görseli zorunludur")]
        public string ImagePath { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // JSON serialize edilmeyecek (zaten view'da kullanmıyoruz)
        public ICollection<Product> Products { get; set; }
    }
}