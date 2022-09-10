using SpicyCatsBlogAPI.Models.Auth;

namespace SpicyCatsBlogAPI.Data.Auth
{
    public interface IAuthRepo
    {
        Task<User> GetUserAsync(string userName);

        Task<string> GetUserId(string userName);

        Task<bool> AddUserAsync(User user);

        void UpdateUser(User user);

        Task<bool> SaveChangesAsync();
    }
}