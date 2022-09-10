using Microsoft.EntityFrameworkCore;
using SpicyCatsBlogAPI.Models.Auth;

namespace SpicyCatsBlogAPI.Data.Auth
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
            if ((await GetUserAsync(user.Username)) != null)
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

        public async Task<string> GetUserId(string userName)
        {
            return (await this.GetUserAsync(userName)).Id;
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