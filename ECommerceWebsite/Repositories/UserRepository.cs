using ECommerceWebsite.Data;
using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceWebsite.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _db;
        private readonly ITokenService _tokenService;
        public UserRepository(DataContext db, ITokenService tokenService)
        {
            _db=db;
            _tokenService=tokenService;
        }

        public Task AddUser(AppUser user)
        {
            _db.Users.Add(user);
            return _db.SaveChangesAsync();

        }

        public async Task<Common> RegisterUser(RegisterDTO model)
        {
           var isphonenumexits= await PhoneNumberExists(model.PhoneNumber);
            if(isphonenumexits)
            {
                return new Common()
                {
                    Message = "Phone Number already Exits",
                    Type = "Error",
                    StatusCode = StatusCodes.Status302Found
                };
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                PhoneNumber = model.PhoneNumber,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password)),
                PasswordSalt = hmac.Key,
                Email = model.Email,
                Username = model.UserName
            };
            await _db.Users.AddAsync(user);
            var issave=await  _db.SaveChangesAsync();
            if(issave > 0)
            {
                return new Common()
                {
                    Message = "Register Successfully",
                    Type = "Success",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                return new Common()
                {
                    Message = "Register Failed",
                    Type = "Error",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }

        public Task<bool> PhoneNumberExists(string phonenumber)
        {
            var result =  _db.Users.AnyAsync(user => user.PhoneNumber == phonenumber);
            return result;             
        }

        public async Task<Common> LoginUser(LoginDTO model)
        {
            var result = await _db.Users.SingleOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);
            if (result == null)
            {
                return new Common()
                {
                    Message = "Invalid Phone Number",
                    Type="Error",
                    StatusCode=StatusCodes.Status302Found
                };
            }
            using var hmac = new HMACSHA512(result.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != result.PasswordHash[i])
                {
                    return new Common()
                    {
                        Message = "Incorrect Password",
                        Type = "Error",
                        StatusCode= StatusCodes.Status404NotFound
                    };
                }
            }
            var userdto = new UserDTO
            {
                PhoneNumber = result.PhoneNumber,
                Token = _tokenService.CreateToken(result)
            };
            return new Common()
            {
                Message = "Logged In successfully",
                Type = "success",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public ActionResult<AppUser> GetUserByPhoneNumberAsync(string mobileNumber)
        {
            var result = _db.Users.FirstOrDefault(user => user.PhoneNumber == mobileNumber);
            return result;
        }

        public ActionResult<AppUser> GetUserByEmailAsync(string email)
        {
            var result = _db.Users.FirstOrDefault(user => user.Email == email);
            return result;
        }

    }
}
