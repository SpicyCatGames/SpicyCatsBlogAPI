﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SpicyCatsBlogAPI.Models.Auth;

namespace SpicyCatsBlogAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // primary key value generation
            var keysProperties = modelBuilder.Model.GetEntityTypes().Select(x => x.FindPrimaryKey()).SelectMany(x => x.Properties);
            foreach (var property in keysProperties)
            {
                property.ValueGenerated = ValueGenerated.OnAdd;
            }

            // enum to string
            modelBuilder
                .Entity<User>()
                .Property(e => e.Role)
                .HasConversion(
                v => v.ToString(),
                v => (Roles)Enum.Parse(typeof(Roles), v));
        }
    }
}

// dotnet tool install --global dotnet-ef
// dotnet tool update --global dotnet-ef

// dotnet add package Microsoft.EntityFrameworkCore.Design