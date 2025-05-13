using Masterpiece.Models;

namespace Masterpiece.ViewModel
{
    public class FilteredProductListViewModel
    {
       
            public List<Product> Products { get; set; } = new();

            public List<Category> Categories { get; set; } = new(); // Corrected to hold Category objects
            public List<string> Colors { get; set; } = new();
            public List<string> Sizes { get; set; } = new();

            public List<int> WishlistProductIds { get; set; } = new();

            // Filters
            public int CategoryId { get; set; }
            public string SelectedColor { get; set; }
            public string SelectedSize { get; set; }
        public string SelectedCategory { get; set; }
        public decimal? MinPrice { get; set; }
            public decimal? MaxPrice { get; set; }
        public int? MinRating { get; set; }


    }
}
