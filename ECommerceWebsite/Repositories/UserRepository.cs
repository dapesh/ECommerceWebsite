using ECommerceWebsite.Data;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebsite.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _db;
        public UserRepository(DataContext db)
        {
            _db=db;
        }

        public Task AddUser(AppUser user)
        {
            _db.Users.Add(user);
            return _db.SaveChangesAsync();

        }

        public Task RegisterUser(AppUser user)
        {
            _db.Users.Add(user);
            return  _db.SaveChangesAsync();
        }

        public async Task<bool> UserExists(string username)
        {
            var result = await _db.Users.AnyAsync(user => user.UserName == username.ToLower());
            return result;             
        }

    }
}
