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
            AppUser user = new AppUser();
            var userphonenumber = _unitOfWork.TokenService.GetUserDetailsFromToken("mobilephone");
            var userDetails = _unitOfWork.UserRepository.GetUserByPhoneNumberAsync(userphonenumber);
            var AppUser = new AppUser()
            {
                Username = userDetails.Value.Username,
                Email = userDetails.Value.Email,
                PhoneNumber = userDetails.Value.PhoneNumber
            };
            return View(AppUser);
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            var result = _unitOfWork.UserRepository.UploadUserImage(file);

            return View(result);
        }
    }
}
