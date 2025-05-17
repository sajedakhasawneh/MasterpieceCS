namespace Masterpiece.ViewModel
{
    public class ProductReviewViewModel
    {
          public string ProductName { get; set; }
            public string ProductImage { get; set; }
            public string CategoryName { get; set; }

            public string UserName { get; set; }

            public int Rating { get; set; }
            public string? Comment { get; set; }
            public DateTime? CreatedAt { get; set; }
        

    }
}
