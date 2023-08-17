﻿using ECommerceWebsite.DTOs;
using ECommerceWebsite.Interface;
using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Mvc;

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
            if (user.StatusCode == StatusCodes.Status200OK)
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
                var res = await _mailService.SendEmailAsync(request1);
                TempData["Message"] = res.Message;
                TempData["Type"] = res.Type;
                TempData["Email"] = res.Email;

                if (res.StatusCode == StatusCodes.Status404NotFound)
                {
                    return RedirectToAction("ForgotPassword", "Account");
                }
                return RedirectToAction("SendOpt", new
                {
                    Email = TempData["Email"],
                    Message = TempData["Message"],
                    Type = TempData["Type"]
                });
                //return Ok(new { Message = "Email sent successfully." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "An error occurred while sending the email.", Error = ex.Message });
            }
        }

        public ActionResult SendOpt(string email, string message, string type)
        {
            var Email =TempData["Email"];
            var Message =TempData["Message"];
            var Type = TempData["Type"];
            return View();
        }
        public async Task<ActionResult> VerifyOtp(string otp, string email)
        {
            TempData["Email"] = email;
            var result = await _unitOfWork.UserRepository.GetUserNameForOtpVerification(otp, email);
            if (result.StatusCode == StatusCodes.Status200OK)
            {
                return View();
            }
            else
            {
                TempData["message"] = result.Message;
                TempData["Type"]= result.Type;
                return RedirectToAction("SendOpt");
            }
        }
        [HttpPost]
        public async Task<ActionResult> UpdatePasssword(string email, string password, string confirmPassword)
        {

            var output = await _unitOfWork.UserRepository.ChangePassword(email, password);

            if (output.StatusCode == StatusCodes.Status200OK)
            {
                TempData["Message"] = output.Message;
                TempData["Status"] = output.StatusCode;
                TempData["Type"]=output.Type;
                return RedirectToAction("Login");
            }



            else
            {
                return RedirectToAction("VerifyOtp", "Account");
            }
        }
    }
}

