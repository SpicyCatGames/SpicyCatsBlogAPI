using System.ComponentModel.DataAnnotations;

namespace SpicyCatsBlogAPI.Models.Auth
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Roles Role { get; set; }

        public string RefreshToken { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }

    public enum Roles
    {
        Admin,
        User
    }
}