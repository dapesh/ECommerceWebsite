﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebsite.Models
{
    public class UserPhoto
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int AppUserId { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
        public string PhotoUrl { get; set; } // Cloudinary photo URL
        public DateTime Created { get; set; } = DateTime.Now;
        public virtual AppUser User { get; set; }
    }
}
