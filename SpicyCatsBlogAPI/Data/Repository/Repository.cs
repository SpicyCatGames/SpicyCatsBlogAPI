using Microsoft.EntityFrameworkCore;
using SpicyCatsBlogAPI.Models.Auth;
using SpicyCatsBlogAPI.Models.Content;

namespace SpicyCatsBlogAPI.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _ctx;

        public Repository(AppDbContext appDbContext)
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

        public async Task<User> GetUserWPostsAsync(string userName)
        {
            // get users without casematching with db side eval
            var nameMatchedUsers =
                await _ctx.Users
                .Where(user => user.Username.Equals(userName))
                .Include(u => u.Posts)
                .ToListAsync();

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

        // posts
        public async Task<List<Post>> GetPostsAsync(int pageNum, int postsPerPage, string category)
        {
            int skipAmout = postsPerPage * (pageNum - 1);

            var query = _ctx.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category.Equals(category));
            }

            query = query.Include(post => post.User);

            return await query.Skip(skipAmout).Take(postsPerPage).ToListAsync();
        }

        public Post GetPost(string id)
        {
            return _ctx.Posts.FirstOrDefault(p => p.Id.Equals(id));
        }

        public int GetPostCount(string category)
        {
            var query = _ctx.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category.Equals(category));
            }

            return query.Count();
        }
    }
}