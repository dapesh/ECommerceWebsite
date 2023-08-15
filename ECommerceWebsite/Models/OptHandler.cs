using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class OtpHandler
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; }
        public string Otp { get; set; }
        public string isVerified { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
