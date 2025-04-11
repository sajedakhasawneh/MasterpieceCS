using Masterpiece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Masterpiece.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext _context;

        public AdminController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult viewCategory()
        {
            var category = _context.Categories.ToList();
            return View(category);
        }

        [HttpPost]
        public IActionResult Category()
        {
            return View();
        }

        public IActionResult editCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            return View(category); // This passes the model to the form
        }


        [HttpPost]
        public IActionResult editCategory(Category category)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory == null)
                return NotFound();

            // Manually update fields
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.ImageUrl = category.ImageUrl;

            _context.SaveChanges();

            return RedirectToAction("viewCategory"); // or wherever you want to go after saving
        }


        public IActionResult addCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addCategory(Category category)
        {
            var newCategory = _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Category));
        }
        public IActionResult deleteCategory(int id) {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("viewCategory");
        }


        ////////////////////////////Product////////////////////////////////////////////////
        public IActionResult viewProduct()
        {
            var product = _context.Products.ToList();
            return View(product);
        }

        public IActionResult addProduct() { 
            return View();
        }
        [HttpPost]
        public IActionResult addProduct(Product product)
        {
            var newProduct = _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(viewProduct));

        }

        public IActionResult editProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product); // This passes the model to the form
        }

        [HttpPost]
        public IActionResult editProduct(Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(c => c.Id == product.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Manually update fields
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.ImageUrl = product.ImageUrl;

            _context.SaveChanges();

            return RedirectToAction("viewProduct"); // or wherever you want to go after saving
        }

        public IActionResult deleteProduct(int id) {
            var product = _context.Categories.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("viewProduct");
        }

    }


    /////////Users////////////////////////////
    ///

}