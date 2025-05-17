using System;
using System.Diagnostics;
using System.Security.Claims;
using Masterpiece.Models;
using Masterpiece.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Masterpiece.Controllers
{
    public class specialOrderController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public specialOrderController(MyDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult specialForm()
        {

            var dbOccasions = _context.SpecialOrders
                .Where(o => o.Occuaion != null)
                .Select(o => o.Occuaion!)
                .Distinct()
                .ToList();

            // fallback to known allowed values
            var fallbackOccasions = new List<string> { "Wedding", "Birthday", "Graduation", "Corporate Gift" };

            var viewModel = new SpecialOrderViewModel
            {
                Categories = _context.Categories.Select(c => c.Name).ToList(),
                Occasions = dbOccasions.Any() ? dbOccasions : fallbackOccasions
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult specialForm(SpecialOrderViewModel model)
        {
            int? userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                // Redirect to login if session is not set
                return RedirectToAction("Register", "User");
            }

            var listItem = new SpecialOrderViewModel
            {
                Occasions = new List<string> { "Wedding", "Birthday", "Graduation", "Corporate Gift" },
                Categories = _context.Categories.Select(c => c.Name).ToList()
            };


            string? imagePath = null;

            if (model.DesignUpload != null && model.DesignUpload.Length > 0)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(model.DesignUpload.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.DesignUpload.CopyTo(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            if (ModelState.IsValid)
            {

                var order = new SpecialOrder
                {
                    UserId = userID ?? 0, // Replace with logged-in user's ID
                    Details = model.DesignDescription ?? "",
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    Img = imagePath,
                    Occuaion = model.ProductOccasion,
                    ProductType = model.SelectedCategory,
                    Delivery = model.NeededBy != null ? DateOnly.FromDateTime(model.NeededBy.Value) : null,
                    Budget = model.MaxBudget, // storing max budget as the primary "Budget" field
                    Price = model.MinBudget ?? 0  // store min budget in "Price"
                };


                _context.SpecialOrders.Add(order);
                 _context.SaveChanges();
            }
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }


    }

}
