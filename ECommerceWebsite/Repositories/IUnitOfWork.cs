using ECommerceWebsite.Services;

namespace ECommerceWebsite.Repositories
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        TokenService TokenService { get; }
    }
}
