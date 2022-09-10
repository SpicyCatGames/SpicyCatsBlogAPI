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

        public async Task AddUserAsync(User user)
        {
            await _ctx.Users.AddAsync(user);
        }

        public async Task<User> GetUserAsync(string userName)
        {
            var nameMatchedUsers = await _ctx.Users.Where(user => user.Username.Equals(userName)).ToListAsync();
            var caseMathcedUser = nameMatchedUsers.First(user => user.Username.Equals(userName));

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
    }
}