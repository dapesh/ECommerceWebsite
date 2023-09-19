using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ECommerceWebsite.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        public UserProfileController(ITokenService tokenService,IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult UserProfileDetails() 
        {
            var userimages = _unitOfWork.UserRepository.GetUsersProfilePicture("userid");
            var appUsers = new List<AppUser>();
            foreach(var userimage in userimages)
            {
                if(userimage != null && userimage.IsMain==true) 
                {
                    ViewBag.ProfilePicture = userimage.PhotoUrl;
                }
                if(userimage != null && userimage.IsMain==false)
                {
                    ViewBag.PhotoList = userimage.PhotoUrl;
                }
                var appUser = new AppUser() 
                {
                    Username = userimage.User.Username,
                    Email = userimage.User.Email,
                    PhoneNumber = userimage.User.PhoneNumber,
                };
                appUsers.Add(appUser);
            }
            
            return View(appUsers);
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            var result = _unitOfWork.UserRepository.UploadUserImage(file);
            return RedirectToAction("UserProfileDetails", result);
        }
    }
}
