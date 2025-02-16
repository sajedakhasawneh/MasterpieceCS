using Microsoft.AspNetCore.Mvc;

namespace Masterpiece.Controllers
{
    public class servicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
