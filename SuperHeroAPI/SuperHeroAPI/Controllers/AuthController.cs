using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SuperHeroAPI.Data;
using SuperHeroAPI.Services.UserService;
using System.Data;
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
        private readonly DataContext context;

        public static User user = new User();
        private readonly IConfiguration configuration;
        private readonly IUserService userService;

        public AuthController(IConfiguration configuration, IUserService userService,DataContext _context)
        {
            this.context = _context;
            this.configuration = configuration;
            this.userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var service = userService.GetMyName();
            return Ok(service);
        }
        [HttpGet("allusers")]
        public async Task<ActionResult<User>> GetAllUser()
        {
            return Ok(await context.users.ToListAsync());
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register (UserDto request)
        {
            User _user = new User();

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            _user.UserName = request.UserName;
            _user.PasswordHash = passwordHash;
            _user.PasswordSalt = passwordSalt;
            _user.Email = request.Email;
            _user.CountryId = request.CountryId;
            _user.Gender = request.Gender;
            _user.RefreshToken = Request.Cookies["refreshToken"];

            if(_user.RefreshToken == null)
            {
                RefreshToken rf = GenerateRefreshToken();
                _user.RefreshToken =Convert.ToString(rf.Token);
            }

                context.users.Add(_user);
            await context.SaveChangesAsync();
            // send api call to database
            //create user object
            //call the database
            return Ok(await context.users.ToListAsync());
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var _user =  context.users.Where(n => n.UserName == request.UserName).FirstOrDefault();

            //get user by name -- call to database
            if (_user ==null ||  _user.UserName != request.UserName)
            {
                return BadRequest("User not Found");
            }
            if (!VerifyPassword(request.Password, _user.PasswordHash, _user.PasswordSalt))
            {
                return BadRequest("Wrong Password");
            }
            var autToken = CreateToken(_user);
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);
            await context.users.AddAsync(_user);

            
            return Ok(_user);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> logout()
        {
            Response.Cookies.Delete("refreshToken");

            return Ok("Success");

        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!user.RefreshToken!.Equals(refreshToken))
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
            await context.SaveChangesAsync();
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

