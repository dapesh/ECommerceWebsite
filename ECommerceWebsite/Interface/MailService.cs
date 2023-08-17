using ECommerceWebsite.Models;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using ECommerceWebsite.Data;

namespace ECommerceWebsite.Interface
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly DataContext _db;
        public MailService(IOptions<MailSettings> mailSettings,DataContext db)
        {
            _mailSettings = mailSettings.Value;
            _db=db;
        }
        public async Task<Common> SendEmailAsync(MailRequest mailRequest)
        {
            var user = _db.Users.FirstOrDefault(x=>x.Email==mailRequest.ToEmail);
            
            if(user == null)
            {
                return new Common()
                {
                    Message = "Invalid Email",
                    Type="error",
                    StatusCode=StatusCodes.Status404NotFound
                };
            }
            else
            {
                var optdetail = _db.OtpManger.Where(x => x.UserName == user.Username && x.isVerified != "y");
                if(optdetail== null)
                {
                    return new Common()
                    {
                        Message = "Invalid OTP",
                        Type = "error",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (optdetail.Count() > 0)
                {
                    foreach (var x in optdetail)
                    {
                        x.isVerified = "n";
                        _db.OtpManger.Update(x);
                        _db.SaveChanges();
                    }
                }
                Random num = new Random(5);
                var data = num.Next();
                OtpHandler opt = new OtpHandler();
                opt.UserName = user.Username;
                opt.isVerified = "p";
                opt.CreateDate = DateTime.UtcNow;
                opt.Otp = data.ToString();
                _db.OtpManger.Add(opt);
                _db.SaveChanges();
                mailRequest.Subject = "Forget Password";
                mailRequest.Body = @$"<h4>Dear {user.Username}</h4><br> <b>Your Otp is {data}</b><br> Regards, <br>{_mailSettings.DisplayName}<br>
                                <h6>Your OTP will expire within 10 minutes. Any Support Contact HeadOffice</h6>";
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return new Common()
                {
                    Message = "OTP Sent Successfully",
                    Type = "success",
                    StatusCode = StatusCodes.Status200OK,
                    Email=user.Email
                };
            }

        }
    }
}
