using Microsoft.EntityFrameworkCore;
using DigitalMenu.Models;

namespace DigitalMenu.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Complaint> Complaints { get; set; }

        public DbSet<SiteVisit> SiteVisits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-LA8IE5R;Database=DigitalMenuDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category tablosu için yapılandırma
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ImagePath).IsRequired();
                entity.HasIndex(e => e.SortOrder);
            });

            // Product tablosu için yapılandırma
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImagePath).IsRequired();
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Calories).IsRequired(false); // Opsiyonel
                entity.HasIndex(e => e.SortOrder);

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Settings tablosu için yapılandırma
            modelBuilder.Entity<Settings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RestaurantName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.WelcomeText).HasMaxLength(500);
                entity.Property(e => e.LogoPath);
            });

            // Varsayılan Settings verisi
            modelBuilder.Entity<Settings>().HasData(new Settings
            {
                Id = 1,
                RestaurantName = "Restoran Adı",
                WelcomeText = "Hoş Geldiniz",
                LogoPath = null,
                UpdatedDate = new DateTime(2024, 1, 1)
            });

            // Complaint tablosu için yapılandırma
            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.HasIndex(e => e.CreatedDate); // Sıralama için index
                entity.HasIndex(e => e.IsRead); // Filtreleme için index
            });

            // SiteVisit tablosu için yapılandırma
            modelBuilder.Entity<SiteVisit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VisitDate).IsRequired();
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.HasIndex(e => e.VisitDate);
            });
        }
    }
}