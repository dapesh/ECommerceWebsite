using ECommerceWebsite.DTOs;
using ECommerceWebsite.Interface;
using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IMailService _mailService;
        public AccountController(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
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
            if (common.StatusCode == StatusCodes.Status200OK)
            {
                TempData["Message"] = common.Message;
                TempData["Type"] = common.Type;
            }
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
        public ActionResult ForgotPassword()
        {
            return View();
        }
       

        [HttpPost]
        public async Task<IActionResult> Send([FromForm] MailRequest request)
        {
            try
            {
                MailRequest request1 = new MailRequest();
                request1.ToEmail = request.ToEmail;
                await _mailService.SendEmailAsync(request1);
                return Ok(new { Message = "Email sent successfully." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "An error occurred while sending the email.", Error = ex.Message });
            }
        }
    }
}

