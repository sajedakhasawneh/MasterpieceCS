using Masterpiece.Models;
using Masterpiece.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Masterpiece.Controllers
{
    public class OwnerController : Controller
    {
        private readonly MyDbContext _context;

        public OwnerController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult viewProduct()
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            //var role = HttpContext.Session.GetString("Role");

            if (userId == null )
            {
                return RedirectToAction("Register", "User"); // or Unauthorized()
            }

            var myProducts = _context.Products
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Category) // if you want to show category names
                .ToList();

            return View(myProducts);
        }


        public IActionResult addProduct()
        {
            var categories = _context.Categories.ToList();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult addProduct(Product product)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Register", "User"); // or handle unauthorized
            }

            // Assign the product owner from session
            product.OwnerId = userId.Value;

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Model error: {entry.Key} => {error.ErrorMessage}");
                    }
                }

                ViewBag.CategoryId = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return View(product);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("viewProduct");
        }

        public IActionResult ViewReviews()
        {
            int? ownerId = HttpContext.Session.GetInt32("UserId");

            //if (ownerId == null)
            //{
            //    return RedirectToAction("Register", "User"); // Or Unauthorized
            //}

            // Get all reviews where the product belongs to this owner
            var reviews = _context.Reviews
                .Where(r => r.Product.OwnerId == ownerId)
                .Select(r => new
                {
                    ReviewId = r.ReviewId,
                    ProductName = r.Product.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserName = r.User.Name, // Or FullName, adjust to your User model
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            // Optionally, create a view model to pass to the view
            return View(reviews);
        }


        public IActionResult showReviews()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || HttpContext.Session.GetString("UserRole") != "owner")
                return Unauthorized();

            var reviews = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                    .ThenInclude(p => p.Category)
                .Where(r => r.Product.OwnerId == userId)
                .Select(r => new ProductReviewViewModel
                {
                    ProductName = r.Product.Name,
                    ProductImage = r.Product.ImageUrl,
                    CategoryName = r.Product.Category.Name,
                    UserName = r.User.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            return View(reviews);
        }














        public IActionResult editProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            var categories = _context.Categories.ToList();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }



        [HttpPost]
        public IActionResult editProduct(Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct == null)
                return NotFound();

            // Update fields
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.IsActive = product.IsActive; // Enable/Disable checkbox

            _context.SaveChanges();

            return RedirectToAction("viewProduct"); // or wherever the list is shown
        }


        public IActionResult DisableProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            product.IsActive = false;
            _context.SaveChanges();

            return RedirectToAction("viewProduct"); // or viewProduct
        }

    }

}
