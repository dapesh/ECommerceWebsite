using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.DTOs
{
    public class LoginDTO
    {
        [StringLength(14)]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(?:\+977|0)[1-9]\d{7,9}$", ErrorMessage = "Please enter a valid Nepali phone number.")]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 12 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$", ErrorMessage = "The password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string Password { get; set; }
    }
}
