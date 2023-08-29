using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public ICollection<UserRole> UserRoles { get; set; }

    }
}
