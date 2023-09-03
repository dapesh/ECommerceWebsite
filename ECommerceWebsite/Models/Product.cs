using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
    }
}
