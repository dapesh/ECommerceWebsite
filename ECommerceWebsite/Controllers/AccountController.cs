using ECommerceWebsite.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            //_userRepository.UserExists();
            return View();
        }
    }
}
