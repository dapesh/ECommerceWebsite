using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceWebsite.Controllers
{
    [Route("[controller]")]

    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("register")]
       
        public IActionResult Register()
        
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await _userRepository.UserExists(registerDto.Username))
            {
                return BadRequest("Username is already taken");
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            await _userRepository.RegisterUser(user);
            return View();
        }

    }
}
