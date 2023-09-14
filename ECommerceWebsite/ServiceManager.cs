using CloudinaryDotNet;
using ECommerceWebsite.Data;
using ECommerceWebsite.Interface;
using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;

namespace ECommerceWebsite
{
    public static class ServiceManager
    {
        public static IServiceCollection GetServices(this IServiceCollection services,IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            services.AddSingleton(new Cloudinary(new Account(
            cloudinarySettings.CloudName,
            cloudinarySettings.ApiKey,
            cloudinarySettings.ApiSecret
               )));
            services.AddHttpContextAccessor();
            services.AddSession(options => {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<IRepository>(provider => new Repository(connectionString));
            // Add services to the container.
            services.AddControllersWithViews();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                                    ValidateIssuer = false,
                                    ValidateAudience = false,
                                };
                            });

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}
