﻿using ECommerceWebsite.Models;
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
        public DbSet<RoleManager> RoleManagers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

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
