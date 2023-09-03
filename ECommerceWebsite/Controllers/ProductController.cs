using ECommerceWebsite.Data;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _db;
        public ProductController(DataContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddProducts()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddProducts(Product product)
        {
            try
            {
                var result = _db.Products.Add(product);
                _db.SaveChanges();
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
                return View();
        }
    }
}
