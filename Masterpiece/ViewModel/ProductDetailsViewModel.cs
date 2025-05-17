using Masterpiece.Models;

namespace Masterpiece.ViewModel
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public Category Category { get; set; }
        public User User { get; set; }

        public List<Review> Reviews { get; set; }

        public FeedbackInputModel NewFeedback { get; set; }
        public bool UserHasPurchased { get; set; }

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        //public bool CanUserReview { get; set; }  // Based on order history
    }

    public class FeedbackInputModel
    {
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
