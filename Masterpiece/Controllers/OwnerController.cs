using Masterpiece.Models;
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
                return RedirectToAction("Login", "Account"); // or handle unauthorized
            }

            // Assign the product owner from session
            product.OwnerId = userId.Value;

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
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


    }

}
