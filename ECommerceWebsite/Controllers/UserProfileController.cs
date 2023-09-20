using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            List<SelectListItem> dropdownItems = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Profile Picture" },
                new SelectListItem { Value = "0", Text = "Others" },
            };
            ViewBag.DropDownItems = dropdownItems;
            var userimages = _unitOfWork.UserRepository.GetUsersProfilePicture("userid");
            return View(userimages);
        }
        [HttpPost]
        public IActionResult UploadImage(int selectedOption, IFormFile file)
        {
            var result = _unitOfWork.UserRepository.UploadUserImage(selectedOption,file);
            return RedirectToAction("UserProfileDetails", result);
        }
    }
}
