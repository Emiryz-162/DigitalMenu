using DigitalMenu.Models;
using DigitalMenu.ViewModels.DigitalMenu.ViewModels;

namespace DigitalMenu.ViewModels
{
    public class CategoryPageViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductDisplayModel> Products { get; set; }
        public List<CategoryNavModel> AllCategories { get; set; }
    }

    public class ProductDisplayModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int? Calories { get; set; }
        public string ImagePath { get; set; }
        public int SortOrder { get; set; }
    }

    namespace DigitalMenu.ViewModels
    {
        public class CategoryPageViewModel
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public List<ProductDisplayModel> Products { get; set; }
            public List<CategoryNavModel> AllCategories { get; set; }
        }

        public class ProductDisplayModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public int? Calories { get; set; }
            public string ImagePath { get; set; }
            public int SortOrder { get; set; }
        }

        public class CategoryNavModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }  // BUNU EKLE
            public int SortOrder { get; set; }
        }
    }
}