using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;

namespace ECommerceWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenService _tokenService;
        public HomeController(ILogger<HomeController> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }
        //[Authorize]
        public IActionResult Index()
        {
            var result = _tokenService.GetMobilePhoneFromToken();
            var serializedUserDto = TempData["UserDto"] as string;
            var userdto = JsonConvert.DeserializeObject<UserDTO>(serializedUserDto);
            ViewBag.Message = TempData["Message"];
            ViewBag.Type = TempData["Type"];
            TempData.Remove("Message");
            TempData.Remove("Type");
            return View(userdto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}