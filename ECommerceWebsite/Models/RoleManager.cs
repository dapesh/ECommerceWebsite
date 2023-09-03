using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class RoleManager
    {
        [Key]
        public int Id { get; set; }
        public string Role { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
