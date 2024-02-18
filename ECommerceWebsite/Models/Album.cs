using System.ComponentModel.DataAnnotations;

namespace ECommerceWebsite.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsDefaultAlbum { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsMainPicture { get; set; }
        public int AppUserId {  get; set; }
        public List<UserPhoto> UserPhotos { get; set; }
    }
}
