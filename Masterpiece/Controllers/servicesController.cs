using Masterpiece.Models;
using Microsoft.AspNetCore.Mvc;

namespace Masterpiece.Controllers
{
    public class servicesController : Controller
    {
        private readonly MyDbContext _context;
        public servicesController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult category()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult products(int ID)
        {
            var product = _context.Products.Where(category => category.CategoryId == ID).ToList();
            return View(product);
        }
        
        public IActionResult singleProduct(int id)
        {
            var prdoductbyId = _context.Products.Where(product => product.Id == id).ToList();
            return View();
        }

    }
}
