namespace ECommerceWebsite.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string username);
        //Task RegisterUser(AppUser user);
    }
}
