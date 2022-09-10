using Microsoft.EntityFrameworkCore;
using SpicyCatsBlogAPI.Models.Auth;

namespace SpicyCatsBlogAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
    }
}

// dotnet tool install --global dotnet-ef
// dotnet tool update --global dotnet-ef

// dotnet add package Microsoft.EntityFrameworkCore.Design