using Masterpiece.Models;
using Masterpiece.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Masterpiece.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext _context;

        public AdminController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult viewCategory()
        {
            var category = _context.Categories.ToList();
            return View(category);
        }

        [HttpPost]
        public IActionResult Category()
        {
            return View();
        }

        public IActionResult editCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            return View(category); // This passes the model to the form
        }


        [HttpPost]
        public IActionResult editCategory(Category category)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory == null)
                return NotFound();

            // Manually update fields
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.ImageUrl = category.ImageUrl;

            _context.SaveChanges();

            return RedirectToAction("viewCategory"); // or wherever you want to go after saving
        }


        public IActionResult addCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addCategory(Category category)
        {
            var newCategory = _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Category));
        }
        public IActionResult deleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("viewCategory");
        }


        ////////////////////////////Product////////////////////////////////////////////////
        public IActionResult viewProduct()
        {
            var product = _context.Products.ToList();
            return View(product);
        }

        public IActionResult addProduct()
        {
            var categories = _context.Categories.ToList();
            var users = _context.Users.ToList();

            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
            ViewBag.OwnerId = new SelectList(users, "UserId", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult addProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                var categories = _context.Categories.ToList();

                // ✅ Only include users with "Owner" role
                var owners = _context.Users
                    .Where(u => u.Role == "Owner")
                    .ToList();

                ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
                ViewBag.OwnerId = new SelectList(owners, "Id", "Name"); // adjust property names to match your User model

                return View(product);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(viewProduct));
        }



        public IActionResult editProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product); // This passes the model to the form
        }

        [HttpPost]
        public IActionResult editProduct(Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(c => c.Id == product.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Manually update fields
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.ImageUrl = product.ImageUrl;

            _context.SaveChanges();

            return RedirectToAction("viewProduct"); // or wherever you want to go after saving
        }

        public IActionResult deleteProduct(int id)
        {
            var product = _context.Categories.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("viewProduct");
        }

        /////////Users////////////////////////////
        ///

        public IActionResult viewUsers()
        {
            var user = _context.Users.ToList();
            return View(user);
        }


        ///////////////////Order
        ///
        //public IActionResult viewOrders()
        //{
        //    var order = _context.Orders.ToList();
        //    return View(order);
        //}

        public IActionResult viewOrders()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new UserOrderViewModel
                {
                    OrderId = o.OrderId,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    User = o.User,
                    Items = o.OrderItems.Select(oi => new orderHistoryVM
                    {
                        ProductName = oi.Product.Name,
                        ImageUrl = oi.Product.ImageUrl,
                        UnitPrice = oi.Product.Price,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToList();

            return View(orders);
        }


        public IActionResult ViewOrderDetails(int id)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderId == id)
                .Select(o => new UserOrderViewModel
                {
                    OrderId = o.OrderId,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    User = o.User,
                    Items = o.OrderItems.Select(oi => new orderHistoryVM
                    {
                        ProductName = oi.Product.Name,
                        ImageUrl = oi.Product.ImageUrl,
                        UnitPrice = oi.Product.Price,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        public IActionResult editOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            var viewModel = new EditOrderStatusViewModel
            {
                OrderId = order.OrderId,
                Status = order.Status
            };

            return View(viewModel);
        }




        [HttpPost]
        public IActionResult editOrder(EditOrderStatusViewModel model)
        {
            var existingOrder = _context.Orders.FirstOrDefault(c => c.OrderId == model.OrderId);
            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.Status = model.Status;
            _context.SaveChanges();

            return RedirectToAction("viewOrders");
        }



        public IActionResult ContactMessages()
        {
            var messages = _context.ContactUs
                .Include(c => c.User)
                .Select(c => new ContactMessageViewModel
                {
                    Id = c.Id,
                    Subject = c.Subject,
                    Message = c.Message,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UserName = c.User != null ? c.User.Name : "Anonymous"
                })
                .ToList();

            return View(messages);
        }







    }

}