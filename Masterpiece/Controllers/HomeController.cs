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

            if (userId == null)
            {
                // Handle case when user is not logged in (optional)
                ModelState.AddModelError("", "User is not logged in.");
                return View(feedback);
            }

            if (ModelState.IsValid)
            {
                feedback.UserId = userId.Value;
                feedback.CreatedAt = DateTime.Now;
                feedback.Status = "pending";

                _context.ContactUs.Add(feedback);
                _context.SaveChanges();

                return Json(new { success = true, message = "Your message has been sent successfully!" });
            }

            return Json(new { success = false, message = "Invalid data. Please check your inputs." });
        }


        //[HttpPost]
        //public IActionResult Contact([FromBody] ContactU feedback)
        //{
        //    if (feedback == null)
        //    {
        //        return Json(new { success = false, message = "Invalid request data! No data received." });
        //    }

        //    Console.WriteLine($"Received Data - Subject: {feedback.Subject}, Message: {feedback.Message}, UserId: {feedback.UserId}");

        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors)
        //                                      .Select(e => e.ErrorMessage).ToList();
        //        Console.WriteLine("Validation Errors: " + string.Join(", ", errors));

        //        return Json(new { success = false, message = "Invalid data!", errors });
        //    }

        //    feedback.CreatedAt = DateTime.Now;
        //    _context.ContactUs.Add(feedback);
        //    _context.SaveChanges();

        //    return Json(new { success = true, message = "Your message has been sent successfully!" });
        //}

        //public IActionResult feedback()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Feedback(ContactU feedback)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        // Handle case when user is not logged in (optional)
        //        ModelState.AddModelError("", "User is not logged in.");
        //        return View(feedback);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        feedback.UserId = userId.Value;
        //        feedback.CreatedAt = DateTime.Now;
        //        feedback.Status = "pending";

        //        _context.ContactUs.Add(feedback);
        //        _context.SaveChanges();

        //        return RedirectToAction("Index"); // Or wherever you want to redirect
        //    }

        //    return View(feedback);
        //}

        //public IActionResult feedback()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult feedback(ContactU feedback)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (ModelState.IsValid)
        //    {
        //        feedback.UserId=userId.Value;
        //        ViewBag.UserId=userId.Value;
        //        ViewBag.time=DateTime.Now;
        //        feedback.CreatedAt = DateTime.Now;
        //        _context.ContactUs.Add(feedback);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(feedback);
        //}
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
