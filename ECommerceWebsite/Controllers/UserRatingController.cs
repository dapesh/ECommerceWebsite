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
            var claimValue = User.FindFirst("mobilephone")?.Value;
            var result = _db.Users.FirstOrDefault(u=>u.PhoneNumber == claimValue);
            ViewBag.RatedByUserId = result.Id;
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
        public IActionResult UserProfile(int userIdwhomtoRate, double AverageRating) 
        {
            ViewBag.AverageRating = AverageRating;
            var userProfile = _db.Users.FirstOrDefault(x=>x.Id==userIdwhomtoRate);
            return View(userProfile);
        }

        [HttpPost]
        public IActionResult AddRating(string comment, int id, int SelectedRating)
        {
            var claimValue = User.FindFirst("mobilephone")?.Value;
            var result = _db.Users.FirstOrDefault(u => u.PhoneNumber == claimValue);
            var ratings = _db.UserRatings.Where(r => r.RatedUserID == id);
            double averageRating = ratings.Any() ? ratings.Average(r => r.Rating) : 0;
            var results = new UserRating()
            {
                RatedByUserID= result.Id,
                Rating= SelectedRating,
                Comment=comment,
                Timestamp=DateTime.Now,
                RatedUserID=id
            };
            _db.UserRatings.Add(results);
            _db.SaveChanges();
            return RedirectToAction("UserProfile", new { userIdwhomtoRate = results.RatedUserID, AverageRating= averageRating });
        }
    }
}
