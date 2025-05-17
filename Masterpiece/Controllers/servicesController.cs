using Masterpiece.Models;
using Masterpiece.ViewModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Masterpiece.Controllers
{
    public class servicesController : Controller
    {
        private readonly MyDbContext _context;
        public servicesController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult category()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult products(int ID)
        {
            var product = _context.Products.Where(category => category.CategoryId == ID).ToList();
            return View(product);
        }







        //public IActionResult singleProduct(int id)
        //{
        //    var product = _context.Products.FirstOrDefault(p => p.Id == id);
        //    if (product == null) return NotFound();

        //    var reviews = _context.Reviews
        //        .Where(r => r.ProductId == id)
        //        .Include(r => r.User)
        //        .ToList();

        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    bool userHasPurchased = _context.Orders
        //        .Include(o => o.OrderItems)
        //        .Any(o => o.UserId == userId &&
        //                  o.OrderItems.Any(oi => oi.ProductId == id));

        //    var vm = new ProductDetailsViewModel
        //    {
        //        Product = product,
        //        Reviews = reviews,
        //        UserHasPurchased = userHasPurchased,
        //        NewFeedback = new FeedbackInputModel
        //        {
        //            ProductId = product.Id
        //        }
        //    };

        //    return View(vm);
        //}


        public IActionResult singleProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            var reviews = _context.Reviews
                .Where(r => r.ProductId == id)
                .Include(r => r.User)
                .ToList();

            var userId = HttpContext.Session.GetInt32("UserId");
            bool userHasPurchased = _context.Orders
                .Include(o => o.OrderItems)
                .Any(o => o.UserId == userId &&
                          o.OrderItems.Any(oi => oi.ProductId == id));

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var reviewCount = reviews.Count;

            var vm = new ProductDetailsViewModel
            {
                Product = product,
                Reviews = reviews,
                UserHasPurchased = userHasPurchased,
                AverageRating = averageRating,
                ReviewCount = reviewCount,
                NewFeedback = new FeedbackInputModel
                {
                    ProductId = product.Id
                }
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult AddReview(FeedbackInputModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Register", "User");
            }

            // Check if user has purchased the product
            bool userHasPurchased = _context.Orders
                .Include(o => o.OrderItems)
                .Any(o => o.UserId == userId &&
                          o.OrderItems.Any(oi => oi.ProductId == model.ProductId) &&
                          o.Status == "delivered");

            if (!userHasPurchased)
            {
                // Rebuild ViewModel to return the product page
                var product = _context.Products
                    .Include(p => p.Category) // optional
                    .FirstOrDefault(p => p.Id == model.ProductId);

                var reviews = _context.Reviews
                    .Where(r => r.ProductId == model.ProductId)
                    .Include(r => r.User)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                var vm = new ProductDetailsViewModel
                {
                    Product = product,
                    Reviews = reviews,
                    UserHasPurchased = false,
                    NewFeedback = model // pass back the attempted input
                };

                ViewBag.Error = "Only verified buyers can leave a review.";
                return View("singleProduct", vm);
            }

            // Save review
            var review = new Review
            {
                ProductId = model.ProductId,
                UserId = userId.Value,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.Now.Date
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("singleProduct", new { id = model.ProductId });
        }

        public IActionResult getAllProducts(List<int> CategoryIds, List<int> Ratings, int? MaxPrice, string SortOrder)
        { 
        
            // Fetch all products and categories
            var products = _context.Products.Include(p => p.Category).ToList();
            var categories = _context.Categories.ToList();

            // Filter by category
            if (CategoryIds != null && CategoryIds.Any())
            {
                products = products.Where(p => CategoryIds.Contains(p.CategoryId)).ToList();
            }

            // Filter by rating
            if (Ratings != null && Ratings.Any())
            {
                products = products.Where(p => Ratings.Any(r => p.Rating >= r)).ToList();
            }

            // Filter by price
            if (MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= MaxPrice.Value).ToList();
            }

            //// Sort by date
            //if (SortOrder == "new")
            //{
            //    products = products.OrderByDescending(p => p.DateAdded).ToList();
            //}
            //else if (SortOrder == "old")
            //{
            //    products = products.OrderBy(p => p.DateAdded).ToList();
            //}

            var viewModel = new productFiltring
            {
                Products = products,
                Categories = categories,
                SelectedCategoryIds = CategoryIds,
                SelectedRatings = Ratings,
                MaxPrice = MaxPrice,
                //SortOrder = SortOrder
            };

            return View(viewModel);
        
        }




        public IActionResult AllProducts(int categoryId, string color, string size, decimal? minPrice, decimal? maxPrice, int minRating)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (categoryId != 0)
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);

            if (!string.IsNullOrEmpty(color))
                productsQuery = productsQuery.Where(p => p.Color == color);

            if (!string.IsNullOrEmpty(size))
                productsQuery = productsQuery.Where(p => p.Size == size);

            if (minPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            if (minRating != 0)
                productsQuery = productsQuery.Where(p => p.Rating >= minRating);

            // Get all filter values from the database
            var categories = _context.Categories.ToList();
            var colors = _context.Products
                .Where(p => !string.IsNullOrEmpty(p.Color))
                .Select(p => p.Color)
                .Distinct()
                .ToList();
            var sizes = _context.Products
                .Where(p => !string.IsNullOrEmpty(p.Size))
                .Select(p => p.Size)
                .Distinct()
                .ToList();

            var userId = HttpContext.Session.GetInt32("UserId");
            var wishlistProductIds = new List<int>();
            if (userId != null)
            {
                wishlistProductIds = _context.Wishlists
                    .Where(w => w.UserId == userId)
                    .Select(w => w.ProductId)
                    .ToList();
            }

            var vm = new FilteredProductListViewModel
            {
                Products = productsQuery.ToList(),
                CategoryId = categoryId,
                Categories = categories,
                Colors = colors,
                Sizes = sizes,
                WishlistProductIds = wishlistProductIds,
                SelectedColor = color,
                SelectedSize = size,
                MinRating = minRating,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return View(vm);
        }




        public IActionResult Index(List<int> CategoryIds, List<int> Ratings, int? MaxPrice, string SortOrder)
        {

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Register", "User");
            }
            // Fetch all products and categories
            var products = _context.Products.Include(p => p.Category).ToList();
            var categories = _context.Categories.ToList();

            // Filter by category
            if (CategoryIds != null && CategoryIds.Any())
            {
                products = products.Where(p => CategoryIds.Contains(p.CategoryId)).ToList();
            }

            // Filter by rating
            if (Ratings != null && Ratings.Any())
            {
                products = products.Where(p => Ratings.Any(r => p.Rating >= r)).ToList();
            }

            // Filter by price
            if (MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= MaxPrice.Value).ToList();
            }

            //// Sort by date
            //if (SortOrder == "new")
            //{
            //    products = products.OrderByDescending(p => p.DateAdded).ToList();
            //}
            //else if (SortOrder == "old")
            //{
            //    products = products.OrderBy(p => p.DateAdded).ToList();
            //}


            var viewModel = new productFiltring
            {
                Products = products,
                Categories = categories,
                SelectedCategoryIds = CategoryIds,
                SelectedRatings = Ratings,
                MaxPrice = MaxPrice,
                //SortOrder = SortOrder
            };

            return View(viewModel);
        }


    }
}
