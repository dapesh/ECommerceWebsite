using ECommerceWebsite.Data;
using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using ECommerceWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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

            if (userexists != null)
            {
                return new Common()
                {
                    Message = "User Name already Exits",
                    Type = "Error",
                    StatusCode = StatusCodes.Status302Found
                };
            }

            if (useremailexists != null)
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
                var userForRoles = _db.Users.FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber);
                var roleForRoles = _db.RoleManagers.FirstOrDefault(x => x.Role == "User");

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

        public async Task<ActionResult<AppUser>> GetUserByPhoneNumberAsync(string mobileNumber)
        {
            var result = await _db.Users.FirstOrDefaultAsync(user => user.PhoneNumber == mobileNumber);
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
        public List<UserPhoto> GetUsersProfilePicture(string Key)
        {
            var result = _tokenService.GetUserDetailsFromToken(Key);
            var userInfo = _db.Users.AsNoTracking().Include(x=>x.UserPhotos).FirstOrDefault(x=>x.Id == Convert.ToInt32(result));
            return userInfo.UserPhotos.ToList();
        }

        public async Task<Common> UploadUserImage(bool selectedOption, List<IFormFile> files, string albumTitle)
        {
            var userphonenumber = _tokenService.GetUserDetailsFromToken("mobilephone");
            var userDetails = GetUserByPhoneNumberAsync(userphonenumber);

            if (userDetails == null)
            {
                return new Common()
                {
                    Message = "Invalid Phone Number",
                    Type = "Error",
                    StatusCode = StatusCodes.Status302Found
                };
            }

            var username = userDetails.Result.Value.Username;
            var userId = userDetails.Result.Value.Id;

            if (files != null && files.Count > 0)
            {
                // Process each uploaded file in the list
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            if (stream != null && stream.CanRead)
                            {
                                try
                                {
                                    long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                                    var apiUrl = $"https://api.cloudinary.com/v1_1/digo594g6/image/upload?timestamp={timestamp}&upload_preset=sfbot1hn";

                                    using (var httpClient = new HttpClient())
                                    {
                                        var formData = new MultipartFormDataContent();
                                        formData.Add(new StreamContent(stream), "file", file.FileName);
                                        Console.WriteLine(await formData.ReadAsStringAsync());

                                        var response = await httpClient.PostAsync(apiUrl, formData);
                                        if (!response.IsSuccessStatusCode)
                                        {
                                            var responseContent = await response.Content.ReadAsStringAsync();
                                        }
                                        string responseJson = await response.Content.ReadAsStringAsync();
                                        dynamic responseObject = JObject.Parse(responseJson);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            string imageUrl = responseObject.secure_url.ToString();
                                            string publicId = responseObject.public_id.ToString();
                                            bool isMain = (selectedOption == true);

                                            if (isMain == true)
                                            {
                                                var userPhotos = _db.UserPhotos.Where(x => x.AppUserId == userId);
                                                foreach (var photos in userPhotos)
                                                {
                                                    photos.IsMain = false;
                                                }
                                                await _db.SaveChangesAsync();
                                            }

                                            var existingAlbum = await _db.Albums.FirstOrDefaultAsync(a => a.Title == albumTitle);
                                            if (existingAlbum == null)
                                            {
                                                existingAlbum = new Album
                                                {
                                                    Title = albumTitle
                                                };
                                                _db.Albums.Add(existingAlbum);
                                                await _db.SaveChangesAsync();
                                            }

                                            UserPhoto photo = new UserPhoto
                                            {
                                                PhotoUrl = imageUrl,
                                                PublicId = publicId,
                                                AppUserId = userId,
                                                IsMain = isMain,
                                                Created = DateTime.Now,
                                                AlbumId = existingAlbum.Id,
                                                Title = albumTitle,
                                            };

                                            await _db.UserPhotos.AddAsync(photo);
                                            await _db.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid source stream or it's not readable.");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error while copying content: {ex.Message}");
                                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid source stream or it's not readable.");
                            }
                        }
                    }
                }
            }

            return new Common()
            {
                Message = "Photos Uploaded Successfully",
                Type = "Success",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public List<Album> GetDropdownForDefaultAlbum()
        {
            return _db.Albums.Where(a=>a.IsDefaultAlbum==true &&a.IsDeleted==false).ToList();

        }

        public List<Album> GetAlbumDetails(int userid)
        {
            var albumsWithPhotos = _db.Albums.Include(a => a.UserPhotos.Where(s=>s.AppUserId==userid)).ToList();
            return albumsWithPhotos;
        }


    }
}
