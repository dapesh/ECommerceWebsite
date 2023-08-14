using ECommerceWebsite.DTOs;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            var common = await _unitOfWork.UserRepository.RegisterUser(model);
            TempData["Message"] = common.Message;
            TempData["Type"] = common.Type;
            return RedirectToAction("Login", "Account");

        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _unitOfWork.UserRepository.LoginUser(loginDTO);
           
            TempData["Message"] = user.Message;
            TempData["Type"] = user.Type;
            if(user.StatusCode==StatusCodes.Status200OK)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Login", "Account");
        }
    }
}

