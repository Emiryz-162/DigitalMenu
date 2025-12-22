using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DigitalMenu.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Ürün görseli zorunludur")]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string Description { get; set; }

        public int? Calories { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }

        // JSON serialize sırasında circular reference'ı önle
        [JsonIgnore]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}