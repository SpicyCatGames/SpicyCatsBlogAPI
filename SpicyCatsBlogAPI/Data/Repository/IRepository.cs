using SpicyCatsBlogAPI.Models.Auth;
using SpicyCatsBlogAPI.Models.Content;

namespace SpicyCatsBlogAPI.Data.Repository
{
    public interface IRepository
    {
        // user
        Task<User> GetUserAsync(string userName);

        Task<User> GetUserWPostsAsync(string username);

        Task<string> GetUserId(string userName);

        Task<bool> AddUserAsync(User user);

        void UpdateUser(User user);

        Task<bool> SaveChangesAsync();

        Task<User> GetUserbyRefreshAsync(string refreshToken);

        // posts
        Task<List<Post>> GetPostsAsync(int pageNum, int postsPerPage, string category);

        Post GetPost(string id);

        int GetPostCount(string category);
    }
}