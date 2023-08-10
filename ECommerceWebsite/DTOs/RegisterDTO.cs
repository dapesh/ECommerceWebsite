using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string PhoneNumber { get; set; }
        [StringLength(8, MinimumLength = 4)]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
