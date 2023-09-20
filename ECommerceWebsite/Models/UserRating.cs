using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class UserRating
    {
        [Key]
        public int ID { get; set; }
        public int RatedUserID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int RatedByUserID { get; set; }
        public DateTime Timestamp { get; set; }

        public AppUser RatedByUser { get; set; }
    }
}
