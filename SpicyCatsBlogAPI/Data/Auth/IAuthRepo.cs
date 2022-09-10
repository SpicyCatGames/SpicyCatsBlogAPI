﻿using SpicyCatsBlogAPI.Models.Auth;

namespace SpicyCatsBlogAPI.Data.Auth
{
    public interface IAuthRepo
    {
        Task<User> GetUserAsync(string userName);

        Task<int> GetUserId(string userName);

        Task AddUserAsync(User user);

        Task<bool> SaveChangesAsync();
    }
}