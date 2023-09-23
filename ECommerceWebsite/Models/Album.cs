using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public List<UserPhoto> UserPhotos { get; set; }
    }
}
