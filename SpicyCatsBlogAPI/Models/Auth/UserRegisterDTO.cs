using System.ComponentModel.DataAnnotations;

namespace SpicyCatsBlogAPI.Models.Auth
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Username is missing")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is missing")]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters long")]
        public string Password { get; set; }
    }
}