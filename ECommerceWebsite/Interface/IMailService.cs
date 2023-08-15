using ECommerceWebsite.Models;

namespace ECommerceWebsite.Interface
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);

    }
}
