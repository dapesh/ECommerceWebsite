using ECommerceWebsite.Data;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Controllers
{
    public class UserRatingController : Controller
    {
        private readonly DataContext _db;
        public UserRatingController(DataContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SearchUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SearchUser(string userName)
        {
            var ifUserExists=_db.Users.FirstOrDefault(x=>x.Username == userName);
            if (ifUserExists != null)
            {
                var userIdwhomtoRate = ifUserExists.Id;
                return RedirectToAction("UserProfile", new { userIdwhomtoRate });
            }
            return View();
        }
        [HttpGet]
        public IActionResult UserProfile(int userIdwhomtoRate) 
        {
            var userProfile = _db.Users.FirstOrDefault(x=>x.Id==userIdwhomtoRate);
            return View(userProfile);
        }
    }
}
