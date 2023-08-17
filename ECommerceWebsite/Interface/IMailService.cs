using ECommerceWebsite.Models;

namespace ECommerceWebsite.Interface
{
    public interface IMailService
    {
        Task<Common> SendEmailAsync(MailRequest mailRequest);

    }
}
