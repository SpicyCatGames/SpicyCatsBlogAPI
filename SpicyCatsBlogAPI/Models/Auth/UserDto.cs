using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpicyCatsBlogAPI.Models.Auth
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = string.Empty;
        public string JWT { get; set; } = string.Empty;
    }
}