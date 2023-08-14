using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;

namespace ECommerceWebsite.Repositories
{
    public interface IUserRepository
    {
        Task<bool> PhoneNumberExists(string phoneNumber);
        Task<Common> RegisterUser(RegisterDTO user);
        Task<Common> LoginUser(LoginDTO user);
        Task AddUser(AppUser user);

    }
}
