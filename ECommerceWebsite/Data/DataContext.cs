using ECommerceWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebsite.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }
    }
}
