using Microsoft.EntityFrameworkCore;
using SpicyCatsBlogAPI.Models.Auth;
using SpicyCatsBlogAPI.Models.Content;

namespace SpicyCatsBlogAPI.Data
{
    public class AuthRepo : IAuthRepo
    {
        private readonly AppDbContext _ctx;

        public AuthRepo(AppDbContext appDbContext)
        {
            _ctx = appDbContext;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            // user with the given username already exists
            if (await GetUserAsync(user.Username) != null)
            {
                return false;
            }

            await _ctx.Users.AddAsync(user);
            return true;
        }

        public Task<Post> GetPostsAsync(int pageNum, int postsPerPage)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserAsync(string userName)
        {
            // get users without casematching with db side eval
            var nameMatchedUsers = await _ctx.Users.Where(user => user.Username.Equals(userName)).ToListAsync();

            if (nameMatchedUsers.Count == 0)
            {
                return null;
            }

            // casematching with client side eval
            var caseMathcedUser = nameMatchedUsers.FirstOrDefault(user => user.Username.Equals(userName), null);

            return caseMathcedUser;
        }

        public async Task<User> GetUserbyRefreshAsync(string refreshToken)
        {
            // get users without casematching with db side eval
            var users = await _ctx.Users.Where(user => user.RefreshToken.Equals(refreshToken)).ToListAsync();

            if (users.Count == 0)
            {
                return null;
            }

            // casematching with client side eval
            var caseMathcedUser = users.FirstOrDefault(user => user.RefreshToken.Equals(refreshToken), null);

            return caseMathcedUser;
        }

        public async Task<string> GetUserId(string userName)
        {
            return (await GetUserAsync(userName)).Id;
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void UpdateUser(User user)
        {
            _ctx.Users.Update(user);
        }
    }
}