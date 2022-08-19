using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SuperHeroAPI.Services.UserService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SuperHeroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        public static User user = new User();
        private readonly IConfiguration configuration;
        private readonly IUserService userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            this.configuration = configuration;
            this.userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var service = userService.GetMyName();
            return Ok(service);
            //var userName = User?.Identity?.Name;
            //var claimName = User.FindFirstValue(ClaimTypes.Name);
            //var role = User.FindFirstValue(ClaimTypes.Role);
            //return Ok(new { userName, claimName, role });
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register (UserDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt); //
            user.UserName = request.UserName;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // send api call to database

            //create user object

            //call the database
            return Ok(user);
        }

        [HttpPut("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            //get user by name -- call to database
            if (user.UserName != request.UserName)
            {
                return BadRequest("User not Found");
            }
            if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong Password");
            }
            var authtoken = CreateToken(user);
            var reftoken = GenerateRefreshToken();
            SetRefreshToken(reftoken);
            return Ok(authtoken);
            //if (user.UserName == request.UserName)
            //    return Ok("Great Work");
            //if (!VerifyPassword(request.Password, user.PasswordSalt, user.PasswordHash))
            //    return BadRequest("Wrong Password");
            //return BadRequest("User not found");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Token");
            }
            else if(user.TokenCreated < DateTime.Now)
            {
                return Unauthorized("Token Expire");
            }
            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);
            return Ok(token);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieoptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.ExpireDate,
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieoptions);
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.CreatedDate;
            user.TokenCreated = newRefreshToken.ExpireDate;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedDate = DateTime.UtcNow,
                ExpireDate = DateTime.Now.AddDays(7),
            };
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
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
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
