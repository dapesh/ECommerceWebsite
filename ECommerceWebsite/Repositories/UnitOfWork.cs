using ECommerceWebsite.Data;
using ECommerceWebsite.Services;

namespace ECommerceWebsite.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _db;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnitOfWork(DataContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {

            _db = db;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public UserRepository UserRepository => new UserRepository(_db, TokenService);

        public TokenService TokenService => new TokenService(_configuration,_httpContextAccessor);
    }
}
