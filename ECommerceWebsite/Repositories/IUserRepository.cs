using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Repositories
{
    public interface IUserRepository
    {
        Task<bool> PhoneNumberExists(string phoneNumber);
        Task<Common> RegisterUser(RegisterDTO user);
        Task<Common> LoginUser(LoginDTO user);
        Task AddUser(AppUser user);
        Task<ActionResult<AppUser>> GetUserByPhoneNumberAsync(string phoneNumber);
        ActionResult<AppUser> GetUserByEmailAsync(string email);
        Task<Common> GetUserNameForOtpVerification(string otp, string email);
        Task<Common> ChangePassword(string email,string password);

        Task<Common> UploadUserImage(bool selectedOption, List<IFormFile> file, string albumTitle);

        List<UserPhoto> GetUsersProfilePicture(string Key);
        List <Album> GetAlbumDetails(int userid);
        List<Album> GetDropdownForDefaultAlbum();

    }
}
