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
            var albumId = userimages.FirstOrDefault().AlbumId;
            var albumDetails = _unitOfWork.UserRepository.GetAlbumDetails(albumId);
            foreach(var albumName in userimages)
            {
                ViewBag.AlbumName = albumName.Title;
            }
            return View(userimages);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(List<IFormFile> files, int selectedOption, string albumTitle)
        {
            
            var result = await _unitOfWork.UserRepository.UploadUserImage(selectedOption,files,albumTitle);
            return RedirectToAction("UserProfileDetails"/*, result*/);
        }
    }
}
