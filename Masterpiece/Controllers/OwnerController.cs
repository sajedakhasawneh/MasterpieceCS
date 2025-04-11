using Microsoft.AspNetCore.Mvc;

namespace Masterpiece.Controllers
{
    public class OwnerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
