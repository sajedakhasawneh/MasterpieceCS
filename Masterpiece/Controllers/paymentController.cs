using Microsoft.AspNetCore.Mvc;

namespace Masterpiece.Controllers
{
    public class paymentController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult payment()
        {
            return View();
        }

        public IActionResult shipping()
        {
            return View();
        }
    }
}
