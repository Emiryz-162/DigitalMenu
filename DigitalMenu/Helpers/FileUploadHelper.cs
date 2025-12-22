namespace DigitalMenu.Helpers
{
    public static class FileUploadHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public static async Task<string> UploadImageAsync(IFormFile file, string folderName, IWebHostEnvironment webHostEnvironment)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya seçilmedi");

            // Dosya uzantısı kontrolü
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"Sadece {string.Join(", ", AllowedExtensions)} formatları desteklenir");

            // Dosya boyutu kontrolü
            if (file.Length > MaxFileSize)
                throw new ArgumentException($"Dosya boyutu en fazla {MaxFileSize / 1024 / 1024} MB olabilir");

            // Benzersiz dosya adı oluştur
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Klasör yolu
            var uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "images", folderName);

            // Klasör yoksa oluştur
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Dosya yolu
            var filePath = Path.Combine(uploadPath, fileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Veritabanına kaydedilecek relative path
            return $"/images/{folderName}/{fileName}";
        }

        public static void DeleteImage(string imagePath, IWebHostEnvironment webHostEnvironment)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            var filePath = Path.Combine(webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}