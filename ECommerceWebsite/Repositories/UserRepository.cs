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

        public Task<bool> PhoneNumberExists(string phonenumber)
        {
            var result =  _db.Users.AnyAsync(user => user.PhoneNumber == phonenumber);
            return result;             
        }

        public Task GetDataByPhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<AppUser> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var result = await _db.Users.SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return result;
        }
    }
}
