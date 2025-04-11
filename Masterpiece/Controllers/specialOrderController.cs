using System.Diagnostics;
using System.Security.Claims;
using Masterpiece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masterpiece.Controllers
{
    public class specialOrderController : Controller
    {
        private readonly MyDbContext _context;
        public specialOrderController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult specialForm()
        {
           
            return View();
        }

        //[HttpPost]
        //public IActionResult SpecialForm(SpecialOrder specialOrder)
        //{
        //   if (ModelState.IsValid)
        //    {
        //        specialOrder.CreatedAt = DateTime.Now;
        //        _context.SpecialOrders.Add(specialOrder);
        //        _context.SaveChanges();
        //        //Json(new { success = true, message = "Sent successfully!" });
        //    }
        //    //return Json(new { success = false, message = "An error occurred while sending!" });
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult specialForm(SpecialOrder order)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                ModelState.AddModelError("", "User is not logged in.");
                return View(order);
            }

            if (ModelState.IsValid)
            {
                order.UserId = userId.Value;
                order.CreatedAt = DateTime.Now;
                order.Status = "pending";

                _context.SpecialOrders.Add(order);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(order);
        }





    }

}
