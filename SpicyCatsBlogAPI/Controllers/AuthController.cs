using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpicyCatsBlogAPI.Data.Repository;
using SpicyCatsBlogAPI.Models.Auth;
using SpicyCatsBlogAPI.Services.UserService;
using SpicyCatsBlogAPI.Utils.ActionFilters.Validation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SpicyCatsBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string REFRESH_TOKEN = "refreshToken";
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IRepository _repo;

        private CookieOptions _cookieOptions
        {
            get
            {
                return new CookieOptions
                {
                    HttpOnly = true, //httponly so that js can't access it
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                };
            }
        }

        public AuthController(IConfiguration configuration, IUserService userService, IRepository repo)
        {
            _configuration = configuration;
            _userService = userService;
            _repo = repo;
        }

        // None of this method without [Authorize]
        //[HttpGet("GetName"), Authorize]
        //public ActionResult<string> GetMe()
        //{
        //    var userName = _userService.GetName();
        //    return Ok(userName);

        //    //return Ok(
        //    //    new
        //    //    {
        //    //        name1 = User.Identity.Name,
        //    //        name = User.FindFirstValue(ClaimTypes.Name),
        //    //        role = User.FindFirstValue(ClaimTypes.Role)
        //    //    });
        //}

        [HttpPost("register")]
        [ValidateModel]
        public async Task<ActionResult> Register(UserRegisterDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = Roles.User
            };

            bool registrationSuccessful = await _repo.AddUserAsync(user);

            if (!registrationSuccessful)
            {
                return BadRequest("username already exists");
            }
            if (await _repo.SaveChangesAsync())
            {
                return Ok($"Role:{user.Role.ToString()}");
            }
            return BadRequest("registration failed");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = await GetUserFromDb(request.Username);

            if (user == null || !user.Username.Equals(request.Username))
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);

            var refreshToken = GenerateRefreshToken();
            await SetRefreshToken(user, refreshToken);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies[REFRESH_TOKEN];

            if (refreshToken == null || refreshToken.Equals(string.Empty))
            {
                return BadRequest("Invalid Refresh Token");
            }

            var user = await _repo.GetUserbyRefreshAsync(refreshToken);

            if (user == null)
            {
                return BadRequest("User with token does not exist in database");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return BadRequest("Refresh Token Expired");
            }

            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            UserDto dto = new UserDto
            {
                Username = user.Username,
                Role = user.Role.ToString(),
                JWT = token
            };

            await SetRefreshToken(user, newRefreshToken);

            return Ok(dto);
        }

        [HttpGet("logout")]
        public ActionResult Logout()
        {
            // TODO delete from server when hosted on a paid hosting service
            Response.Cookies.Delete(REFRESH_TOKEN, _cookieOptions);
            return Ok();
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(15),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private async Task SetRefreshToken(User user, RefreshToken newRefreshToken)
        {
            var cookieOptions = _cookieOptions;
            cookieOptions.Expires = newRefreshToken.Expires;

            Response.Cookies.Append(REFRESH_TOKEN, newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;

            _repo.UpdateUser(user);
            await _repo.SaveChangesAsync();
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value
                )
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private async Task<User> GetUserFromDb(string username) =>
            await _repo.GetUserAsync(username);
    }
}