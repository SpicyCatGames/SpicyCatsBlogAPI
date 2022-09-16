﻿using SpicyCatsBlogAPI.Models.Auth;
using SpicyCatsBlogAPI.Models.Content;

namespace SpicyCatsBlogAPI.Data
{
    public interface IAuthRepo
    {
        // user
        Task<User> GetUserAsync(string userName);

        Task<string> GetUserId(string userName);

        Task<bool> AddUserAsync(User user);

        void UpdateUser(User user);

        Task<bool> SaveChangesAsync();

        Task<User> GetUserbyRefreshAsync(string refreshToken);

        // posts
        Task<Post> GetPostsAsync(int pageNum, int postsPerPage);
    }
}