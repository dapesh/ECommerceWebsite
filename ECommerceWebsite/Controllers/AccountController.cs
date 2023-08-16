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
        private readonly ITokenService _tokenService;

        public AccountController(IUnitOfWork unitOfWork, IMailService mailService, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _tokenService = tokenService;
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
                return RedirectToAction("SendOpt", new { email = request.ToEmail});
                //return Ok(new { Message = "Email sent successfully." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "An error occurred while sending the email.", Error = ex.Message });
            }
        }

        public ActionResult SendOpt(string email)
        {
            TempData["Email"] = email;
            return View();
        }
        public async Task<ActionResult> VerifyOtp(string otp,string email)
        {
             TempData["Email"] =email;
            var result = await _unitOfWork.UserRepository.GetUserNameForOtpVerification(otp,email);
            if(result.StatusCode ==StatusCodes.Status200OK)
            {

                return View();
            }
            else
            {
                TempData["message"]=result.Message;
                return RedirectToAction("SendOpt");
            }
        }
        [HttpPost]
        public ActionResult UpdatePasssword(string email,string password)
        {
            
            return RedirectToAction("Login");
        }
    }
}

