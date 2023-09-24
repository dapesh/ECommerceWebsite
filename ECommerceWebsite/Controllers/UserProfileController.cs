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
            var defaultAlbums = _unitOfWork.UserRepository.GetDropdownForDefaultAlbum();
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
         


            return View(albumsWithPhotos);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(List<IFormFile> files, bool selectedOption, string albumTitle)
        {
            
            var result = await _unitOfWork.UserRepository.UploadUserImage(selectedOption,files,albumTitle);
            return RedirectToAction("UserProfileDetails"/*, result*/);
        }
    }
}
