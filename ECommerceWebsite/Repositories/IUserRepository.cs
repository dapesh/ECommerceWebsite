using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;

namespace ECommerceWebsite.Repositories
{
    public interface IUserRepository
    {
        Task<bool> PhoneNumberExists(string phoneNumber);
        Task RegisterUser(AppUser user);
        Task<AppUser> GetUserByPhoneNumberAsync(string phoneNumber);
        Task AddUser(AppUser user);

    }
}
