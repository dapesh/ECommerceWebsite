using System.Data;

namespace ECommerceWebsite.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public AppUser User { get; set; }

        public int RoleId { get; set; }
        public RoleManager Role { get; set; }
    }
}
