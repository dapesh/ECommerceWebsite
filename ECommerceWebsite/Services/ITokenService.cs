using ECommerceWebsite.Models;

namespace ECommerceWebsite.Services
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
        string GetMobilePhoneFromToken();
    }
}
