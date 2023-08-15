using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Registered Email Address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }

    }
}
