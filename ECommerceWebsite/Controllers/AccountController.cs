using ECommerceWebsite.DTOs;
using ECommerceWebsite.Interface;
using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Authentication;
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
            var districts = new Dictionary<string, string>
            {
            { "Achham", "Achham" }, { "Arghakhanchi", "Arghakhanchi" }, { "Baglung", "Baglung" },
            { "Baitadi", "Baitadi" }, { "Bajhang", "Bajhang" }, { "Bajura", "Bajura" },
            { "Banke", "Banke" }, { "Bara", "Bara" }, { "Bardiya", "Bardiya" },
            { "Bhaktapur", "Bhaktapur" }, { "Bhojpur", "Bhojpur" }, { "Chitwan", "Chitwan" },
            { "Dadeldhura", "Dadeldhura" }, { "Dailekh", "Dailekh" }, { "Dang Deukhuri", "Dang Deukhuri" },
            { "Darchula", "Darchula" }, { "Dhading", "Dhading" }, { "Dhankuta", "Dhankuta" },
            { "Dhanusa", "Dhanusa" }, { "Dholkha", "Dholkha" }, { "Dolpa", "Dolpa" },
            { "Doti", "Doti" }, { "Gorkha", "Gorkha" }, { "Gulmi", "Gulmi" }, { "Humla", "Humla" },
            { "Ilam", "Ilam" }, { "Jajarkot", "Jajarkot" }, { "Jhapa", "Jhapa" }, { "Jumla", "Jumla" },
            { "Kailali", "Kailali" }, { "Kalikot", "Kalikot" }, { "Kanchanpur", "Kanchanpur" },
            { "Kapilvastu", "Kapilvastu" }, { "Kaski", "Kaski" }, { "Kathmandu", "Kathmandu" },
            { "Kavrepalanchok", "Kavrepalanchok" }, { "Khotang", "Khotang" }, { "Lalitpur", "Lalitpur" },
            { "Lamjung", "Lamjung" }, { "Mahottari", "Mahottari" }, { "Makwanpur", "Makwanpur" },
            { "Manang", "Manang" }, { "Morang", "Morang" }, { "Mugu", "Mugu" }, { "Mustang", "Mustang" },
            { "Myagdi", "Myagdi" }, { "Nawalparasi", "Nawalparasi" }, { "Nuwakot", "Nuwakot" },
            { "Okhaldhunga", "Okhaldhunga" }, { "Palpa", "Palpa" }, { "Panchthar", "Panchthar" },
            { "Parbat", "Parbat" }, { "Parsa", "Parsa" }, { "Pyuthan", "Pyuthan" },
            { "Ramechhap", "Ramechhap" }, { "Rasuwa", "Rasuwa" }, { "Rautahat", "Rautahat" },
            { "Rolpa", "Rolpa" }, { "Rukum", "Rukum" }, { "Rupandehi", "Rupandehi" },
            { "Salyan", "Salyan" }, { "Sankhuwasabha", "Sankhuwasabha" }, { "Saptari", "Saptari" },
            { "Sarlahi", "Sarlahi" }, { "Sindhuli", "Sindhuli" }, { "Sindhupalchok", "Sindhupalchok" },
            { "Siraha", "Siraha" }, { "Solukhumbu", "Solukhumbu" }, { "Sunsari", "Sunsari" },
            { "Surkhet", "Surkhet" }, { "Syangja", "Syangja" }, { "Tanahu", "Tanahu" },
            { "Taplejung", "Taplejung" }, { "Terhathum", "Terhathum" }, { "Udayapur", "Udayapur" }
            };
            ViewBag.Districts = districts;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            var selectedDistrict = "Kathmandu";//model.SelectedDistrict;
            var common = await _unitOfWork.UserRepository.RegisterUser(model);
            if (common.StatusCode == StatusCodes.Status200OK)
            {
                TempData["Message"] = common.Message;
                TempData["Type"] = common.Type;
            }
            else if(common.StatusCode==StatusCodes.Status302Found)
            {
                TempData["Message"] = common.Message;
                TempData["Type"] = common.Type;
                return View();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return RedirectToAction("Login", "Account");
        }
    }
}

