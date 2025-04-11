using Masterpiece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                return RedirectToAction(nameof(Login));

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
            return View();
        }

        //[HttpPost]
        //public IActionResult Login(string email, string password)
        //{
        //    // Attempt to find the user by email and password
        //    var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

        //    // Check if user is found
        //    if (user == null)
        //    {
        //        // email not found
        //        ViewBag.Error = "Email not found.";
        //        return RedirectToAction("Register"); // this stays on the login view
        //    }
        //    else if (user.Password != password)
        //    {
        //        // password mismatch
        //        ViewBag.Error = "Invalid password.";
        //        return View();
        //    }

        //    // Login successful
        //    else
        //    {
        //        // Store the user's ID in the session
        //        HttpContext.Session.SetInt32("UserId", user.Id);

        //        // Redirect based on the user's role
        //        if (user.Role == "customer")
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else if (user.Role == "owner")
        //        {
        //            return RedirectToAction("Owner", "Index");
        //        }
        //        else
        //        {
        //            return RedirectToAction("", "Admin");
        //        }
        //    }
        //}


        //public IActionResult Profile() {
        //    int? userID = HttpContext.Session.GetInt32("UserID");
        //    var user = _context.Users.Find(userID);
        //    return View(user);
        //}

        public IActionResult Profile()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");

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
                return RedirectToAction("Profile", "Home");
            }

            if (user.Role == "owner")
            {
                return RedirectToAction("Index", "Owner");
            }
            else { 
                return RedirectToAction("Index", "Admin");
            }
            return View(user);
        }

        //public IActionResult Profile()
        //{
        //    if (HttpContext.Session.GetInt32("UserId") == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var userId = HttpContext.Session.GetInt32("UserId").Value;
        //    var userType = HttpContext.Session.GetString("UserType");

        //    if (userType == "HR")
        //    {
        //        var hrUser = _context.Hrs.Find(userId);
        //        return View(hrUser);
        //    }
        //    else if (userType == "Manager")
        //    {
        //        var managerUser = _context.Managers
        //                                  .Include(m => m.Department)
        //                                  .FirstOrDefault(m => m.Id == userId);
        //        return View(managerUser);
        //    }
        //    else if (userType == "Employee")
        //    {
        //        var employeeUser = _context.Employees
        //                                   .Include(e => e.Department)
        //                                   .Include(e => e.Manager)
        //                                   .FirstOrDefault(e => e.Id == userId);
        //        return View(employeeUser);
        //    }

        //    return RedirectToAction("Login");
        //}

        //public IActionResult editProfile()
        //{
        //    int? userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        // User not logged in
        //        return RedirectToAction("Login", "User");
        //    }

        //    var user = _context.Users.Find(userId);

        //    if (user == null)
        //    {
        //        return NotFound("User not found");
        //    }

        //    return View(user);
        //}
        public IActionResult editProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return View(user);
        }


        //[HttpPost]
        //public IActionResult editProfile(User userUpdate)
        //{
        //    int? userId = HttpContext.Session.GetInt32("UserId");
        //    var user = _context.Users.Find(userId);

        //    if (user != null)
        //    {
        //        user.Name = userUpdate.Name;
        //        user.Email = userUpdate.Email;
        //        user.Password = userUpdate.Password;
        //        //user.City = userUpdate.ProfileImage;
        //        user.Phone = userUpdate.Name;
        //        _context.SaveChanges();
        //    }
        //    return RedirectToAction("Profile");

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult editProfile(User userUpdate)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (ModelState.IsValid)
            {
                user.Name = userUpdate.Name;
                user.Email = userUpdate.Email;

                // Only update password if not empty
                if (!string.IsNullOrWhiteSpace(userUpdate.Password))
                {
                    user.Password = userUpdate.Password; // 🔐 Hash before saving in real apps!
                }

                user.Phone = userUpdate.Phone;
                // If there's a profile image or other fields, update here.

                _context.SaveChanges();

                return RedirectToAction("Profile");
            }

            return View(userUpdate);
        }



        public IActionResult resetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult resetPassword(User user)
        {
            return View();
        }
    }
}
