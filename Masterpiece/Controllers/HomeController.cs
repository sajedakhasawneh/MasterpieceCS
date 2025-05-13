using System.Diagnostics;
using Masterpiece.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Environment;

namespace Masterpiece.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;

        public HomeController(MyDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult contact(ContactU feedback)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            //if (userId == null)
            //{
            //    // Handle case when user is not logged in (optional)
            //    ModelState.AddModelError("", "User is not logged in.");
            //    return View();
            //}

            if (ModelState.IsValid)
            {
                if (userId != null)
                {
                    feedback.UserId = userId.Value;
                }
                //feedback.UserId = userId.Value;
                feedback.CreatedAt = DateTime.Now;
                feedback.Status = "pending";

                _context.ContactUs.Add(feedback);
                _context.SaveChanges();

                return Json(new { success = true, message = "Your message has been sent successfully!" });
            }

            return Json(new { success = false, message = "Invalid data. Please check your inputs." });
        }




       
        public IActionResult about()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
