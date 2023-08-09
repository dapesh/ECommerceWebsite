using ECommerceWebsite.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerceWebsite.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _db;
        public UserRepository(DataContext db)
        {
            _db=db;
            //return await _db.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

        public Task<bool> UserExists(string username)
        {
            throw new NotImplementedException();
        }
    }
}
