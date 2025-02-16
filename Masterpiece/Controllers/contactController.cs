using Microsoft.AspNetCore.Mvc;

namespace Masterpiece.Controllers
{
    public class contactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
