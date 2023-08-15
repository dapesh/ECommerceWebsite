﻿using ECommerceWebsite.Data;
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

            services.AddHttpContextAccessor();
            services.AddSession(options => {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });
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
