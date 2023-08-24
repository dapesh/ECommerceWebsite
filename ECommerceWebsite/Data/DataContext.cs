using ECommerceWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebsite.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<ExcelDataModel> ExcelUpload {  get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<OtpHandler> OtpManger { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExcelDataModel>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<OtpHandler>().Property(x=>x.Otp).HasMaxLength(50);
            modelBuilder.Entity<OtpHandler>().Property(x=>x.isVerified).HasMaxLength(1);
            modelBuilder.Entity<OtpHandler>().HasKey(x => x.Id);
            modelBuilder.Entity<OtpHandler>().Property(x => x.UserName).HasMaxLength(50);
            base.OnModelCreating(modelBuilder);
        }
    }
}
