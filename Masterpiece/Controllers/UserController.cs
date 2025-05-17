using System.Diagnostics;
using Masterpiece.Models;
using Masterpiece.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Masterpiece.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            ViewBag.RoleList = new SelectList(new List<string> { "customer", "owner" });
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            ViewBag.RoleList = new SelectList(new List<string> { "customer", "owner" });
            if (ModelState.IsValid)
            { 
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Register));

            }
            else
            {
                return View(user);
            }
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User loginUser)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == loginUser.Email && u.Password == loginUser.Password); // 🔐 hash this in real apps

            if (user != null)
            {
                // Set user info in session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role); // e.g., "Admin", "Customer", "Artist"

                // Redirect based on role
                switch (user.Role)
                {
                    case "admin":
                        return RedirectToAction("Index", "Admin");
                    case "customer":
                        return RedirectToAction("Index", "Home");
                    case "owner":
                        return RedirectToAction("Index", "Owner");
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return RedirectToAction(nameof(Register));
        }

       
        public IActionResult Profile()
        {
            int? userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                // Redirect to login if session is not set
                return RedirectToAction("Register", "User");
            }

            var user = _context.Users.Find(userID);

            if (user == null)
            {
                // Handle case where user is not found in the database
                return NotFound("User not found.");
            }

            if (user.Role == "customer"){
                return View(user);
            }

            if (user.Role == "owner")
            {
                return RedirectToAction("Index", "Owner");
            }
            else { 
                return RedirectToAction("Index", "Admin");
            }
        }

       
        public IActionResult editProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Register", "User");
            }

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return View(user);
        }


        [HttpPost]
        public IActionResult editProfile(User userUpdate)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.Name = userUpdate.Name;
                user.Email = userUpdate.Email;
                //user.Password = userUpdate.Password;
                user.City = userUpdate.City;
                user.Phone = userUpdate.Phone;

                _context.SaveChanges(); 
                return RedirectToAction(nameof(Profile));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }

        
        public IActionResult resetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult resetPassword(changePasswordVM password)
        {
      
                if (!ModelState.IsValid)
                {
                    return View(password);
                }

                // Step 1: Get the logged-in user's ID
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login");
                }

                // Step 2: Find the user
                var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
                if (user == null)
                {
                    return NotFound();
                }

                // Step 3: Check if the current password matches
                if (user.Password != password.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "The current password is incorrect.");
                    return View(password);
                }

                // Step 4: Update the password
                user.Password = password.NewPassword;
                _context.SaveChanges();

            TempData["ShowSuccessModal"] = true;
            return RedirectToAction("Profile");
        
        }

        public IActionResult orderHistory(string sort = "desc")
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Register", "User");

            // Start building the query
            IQueryable<Order> ordersQuery = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product);

            // Apply sorting AFTER includes
            ordersQuery = sort == "asc"
                ? ordersQuery.OrderBy(o => o.CreatedAt)
                : ordersQuery.OrderByDescending(o => o.CreatedAt);

            // Execute query
            var orders = ordersQuery.ToList();

            // Map to ViewModel
            var viewModel = orders.Select(o => new UserOrderViewModel
            {
                OrderId = o.OrderId,
                CreatedAt = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new orderHistoryVM
                {
                    ProductName = oi.Product.Name,
                    ImageUrl = oi.Product.ImageUrl,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            return View(viewModel);
        }








        public IActionResult addToWishlist()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Register", "User");
            }

            var wishlistItems = _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ToList();

            // Optional: map to a ViewModel
            var products = wishlistItems.Select(w => w.Product).ToList();
            return View(products);
        }


        [HttpPost]
        public IActionResult addToWishlist(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Register", "User");
            }

            // Check if the product is already in wishlist
            var existing = _context.Wishlists
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (existing == null)
            {
                var wishlistItem = new Wishlist
                {
                    UserId = userId.Value,
                    ProductId = productId
                };

                _context.Wishlists.Add(wishlistItem);
                _context.SaveChanges();
            }

            return RedirectToAction("addToWishlist");
        }



        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            var Item = _context.Wishlists.FirstOrDefault(ci => ci.Product.Id == id);

            if (Item != null)
            {
                _context.Wishlists.Remove(Item);
                _context.SaveChanges();
            }

            return RedirectToAction("addToWishlist", "User");
        }


        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            return RedirectToAction("Register");
        }


    }
}
