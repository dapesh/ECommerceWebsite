using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
       
        public IActionResult Register()       
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Type = TempData["Type"];
            TempData.Remove("Message");
            TempData.Remove("Type");
            var model = new RegisterDTO();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _userRepository.PhoneNumberExists(model.PhoneNumber))
                {
                    TempData["Message"] = "This Phone Number already taken. Please choose the different one.";
                    TempData["Type"] = "error";
                    return RedirectToAction("Register");
                }
                using var hmac = new HMACSHA512();
                var user = new AppUser
                {
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password)),
                    PasswordSalt = hmac.Key,
                    Email = model.Email,
                    Username = model.UserName
                };
                await _userRepository.RegisterUser(user);
                TempData["Message"] = "Registered successfully";
                TempData["Type"] = "success";
            }
            return RedirectToAction("Login", "Account");

        }

        public IActionResult Login()        
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Type = TempData["Type"];
            TempData.Remove("Message");
            TempData.Remove("Type");
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByPhoneNumberAsync(loginDTO.PhoneNumber);
            if (user == null)
            {
                TempData["Message"] = "Invalid Phone Number";

            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    TempData["Message"] = "Incorrect Password";
                    TempData["Type"] = "error";
                    return RedirectToAction("Login", "Account");
                }
            }

            TempData["Message"] = "Logged In successfully";
            TempData["Type"] = "success";
            return RedirectToAction("Index","Home");
        }
    }
}

