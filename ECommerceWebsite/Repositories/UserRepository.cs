using ECommerceWebsite.Data;
using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            _db = db;
            _tokenService = tokenService;
        }

        public Task AddUser(AppUser user)
        {
            _db.Users.Add(user);
            return _db.SaveChangesAsync();

        }

        public async Task<Common> RegisterUser(RegisterDTO model)
        {
            var isphonenumexits = await PhoneNumberExists(model.PhoneNumber);
            var userexists = await _db.Users.FirstOrDefaultAsync(x => x.Username == model.UserName);
            var useremailexists = await _db.Users.FirstOrDefaultAsync(x => x.Email == model.Email);

            var username = model.UserName;
            if(username != null)
            {
                
            }
            if(userexists != null) 
            {
                return new Common()
                {
                    Message = "User Name already Exits",
                    Type = "Error",
                    StatusCode = StatusCodes.Status302Found
                };
            }

            if(useremailexists != null)
            {
                return new Common()
                {
                    Message = "User Email already Exits",
                    Type = "Error",
                    StatusCode = StatusCodes.Status302Found
                };
            }
            if (isphonenumexits)
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
            

            var issave = await _db.SaveChangesAsync();
            if (issave > 0)
            {
                var roleManager = new RoleManager { Role = "User", Name = username };
                await _db.RoleManagers.AddAsync(roleManager);
                await _db.SaveChangesAsync();

                var userForRoles = _db.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
                var roleForRoles = _db.RoleManagers.FirstOrDefault(x => x.Name == model.UserName);

                var userRole = new UserRole
                {
                    UserId = userForRoles.Id,
                    RoleId = roleForRoles.Id
                };
                _db.UserRoles.Add(userRole);
                _db.SaveChanges();
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
            var result = _db.Users.AnyAsync(user => user.PhoneNumber == phonenumber);
            return result;
        }

        public async Task<Common> LoginUser(LoginDTO model)
        {
            var result = await _db.Users.SingleOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);
            var userName = result.Username;
            var roleDetails =  _db.RoleManagers.FirstOrDefault(x => x.Name == userName);
            if (roleDetails != null)
            {
                var role = roleDetails.Role;
                if (role == "User")
                {
                    return new Common()
                    {
                        Message = "You are Unauthorized to Login",
                        Type="Error",
                        StatusCode= StatusCodes.Status302Found
                    };
                }
            }
            if (result == null)
            {
                return new Common()
                {
                    Message = "Invalid Phone Number",
                    Type = "Error",
                    StatusCode = StatusCodes.Status302Found
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
                        StatusCode = StatusCodes.Status404NotFound
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

        public async Task<Common> GetUserNameForOtpVerification(string otp, string email)
        {
            var result = await _db.Users.FirstOrDefaultAsync(a => a.Email == email);
            var username = result.Username;
            var OTP = await _db.OtpManger.FirstOrDefaultAsync(x => x.Otp == otp && username == x.UserName && x.isVerified == "p");

            if(OTP!=null)
            {
                if (OTP.CreateDate.AddMinutes(10) > DateTime.UtcNow)
                {
                    return new Common()
                    {
                        Message = "OTP Verified Successfully",
                        Type = "success",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
            }
            return new Common()
            {
                Message = "Invalid OTP or Email",
                Type = "error",
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        public async Task<Common> ChangePassword(string Email, string password)
        {
            try
            {
                var result = await _db.Users.FirstOrDefaultAsync(x => x.Email == Email);
                if (result == null)
                {
                    return new Common()
                    {
                        Message = "Invalid Email Address",
                        Type = "error",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                using var hmac = new HMACSHA512();
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                var passwordSalt = hmac.Key;
                var email = Email;
                result.PasswordHash = passwordHash;
                result.PasswordSalt = passwordSalt;
                var issave = await _db.SaveChangesAsync();

                if (issave > 0)
                {
                    return new Common()
                    {
                        Message = "Password Changed Successfully",
                        Type = "Success",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                else
                {
                    return new Common()
                    {
                        Message = "Password Change Failed",
                        Type = "Error",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new Common()
                {
                    Message = "An error occurred while changing the password.",
                    Type = "Error",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
           
        }
    }
}
