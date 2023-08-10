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
            RegisterDTO model = new();
            ViewBag.Message = TempData["Message"];
            ViewBag.Type = TempData["Type"];
            TempData.Remove("Message");
            TempData.Remove("Type");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            if (await _userRepository.PhoneNumberExists(model.PhoneNumber))
            {
                TempData["Message"] = "Username is already taken";
                TempData["Type"] = "error";
                return RedirectToAction("Register");

            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                PhoneNumber = model.PhoneNumber,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password)),
                PasswordSalt = hmac.Key,
                Email = "Admin@123",
                Username = "Admin"
            };
            await _userRepository.RegisterUser(user);
            TempData["Message"] = "Registered successfully";
            TempData["Type"] = "success";
            return RedirectToAction("Register");
        }

        public IActionResult Login()
        
        {
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

            if (user == null)
            {
                TempData["Message"] = "Invalid Username";
            } 
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    TempData["Message"] = "Incorrect Password";
                }
            }

            TempData["Message"] = "Logged In successfully";
            TempData["Type"] = "success";
            return RedirectToAction("Privacy","Home");
        }
    }
}

