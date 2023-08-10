using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Please enter a valid phone number in the format (XXX) XXX-XXXX.")]
        public string PhoneNumber { get; set; }
        [StringLength(8, MinimumLength = 4)]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
