using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg;

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
            var userPhone = _unitOfWork.TokenService.GetUserDetailsFromToken("mobilephone");
            var appUserId = _unitOfWork.UserRepository.GetUserByPhoneNumberAsync(userPhone).Result.Value.Id;
            var defaultAlbums = _unitOfWork.UserRepository.GetDropdownForDefaultAlbum(appUserId);
            var dropdownItems = defaultAlbums.Select(albums => new SelectListItem
            {
                Value = albums.IsMainPicture.ToString(),
                Text = albums.Title
            }).ToList();
            ViewBag.DropDownItems = dropdownItems;

            var userimages = _unitOfWork.UserRepository.GetUsersProfilePicture("userid");
            var albumsWithPhotos = new List<Album>();
            foreach (var users in userimages)
            {
                var UserId = users.User.Id;
                albumsWithPhotos=_unitOfWork.UserRepository.GetAlbumDetails(UserId);
            }
            ViewBag.appUserId = appUserId;
            return View(albumsWithPhotos);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(List<IFormFile> files, bool selectedOption, string albumTitle)
        {
            var userPhone = _unitOfWork.TokenService.GetUserDetailsFromToken("mobilephone");
            var appUserId = _unitOfWork.UserRepository.GetUserByPhoneNumberAsync(userPhone).Result.Value.Id;

            if(albumTitle != null)
            {
                var resultAlbumTitle =  _unitOfWork.UserRepository.AddAlbumTitle(albumTitle, appUserId);
            }
            var result = await _unitOfWork.UserRepository.UploadUserImage(selectedOption,files,albumTitle);
            return RedirectToAction("UserProfileDetails"/*, result*/);
        }
        public JsonResult GetPhotosByAlbum(int albumId)
        {
            var photos = _unitOfWork.UserRepository.GetPhotosByAlbum(albumId);
            return Json(photos);
        }
        public string AddAlbumTitle(int albumName) 
        {
            return "Success";
        }
        public ActionResult EditUserProfile()
        {
            var userPhone = _unitOfWork.TokenService.GetUserDetailsFromToken("mobilephone");
            var appUserId = _unitOfWork.UserRepository.GetUserByPhoneNumberAsync(userPhone).Result.Value.Id;
            ViewBag.appUserId = appUserId;
            return View();
        }
        [HttpPost]
        public ActionResult EditUserProfile(string appUserId)
        {
            return View();
        }
    }
}
