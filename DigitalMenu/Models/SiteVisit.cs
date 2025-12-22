using System.ComponentModel.DataAnnotations;

namespace DigitalMenu.Models
{
    public class SiteVisit
    {
        public int Id { get; set; }

        [Required]
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }
    }
}