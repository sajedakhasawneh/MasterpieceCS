using System.Collections.Generic;
using Masterpiece.Models;
namespace Masterpiece.ViewModel
{
    public class productFiltring
    {
        // Filters
        public int? CategoryId { get; set; }
        public double? MinRating { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }

        // Data to display
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new(); // for the filter dropdown

        public List<int>? SelectedCategoryIds { get; set; }
        public List<int>? SelectedRatings { get; set; }
        public List<int> WishlistProductIds { get; set; } = new List<int>();

    }


}
