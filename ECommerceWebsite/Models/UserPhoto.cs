using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebsite.Models
{
    public class UserPhoto
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        [ForeignKey("User")]
        public int AppUserId { get; set; }
        [ForeignKey("Album")]
        public int AlbumId { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public AppUser User { get; set; }
        public Album Album { get; set; }
    }
}
