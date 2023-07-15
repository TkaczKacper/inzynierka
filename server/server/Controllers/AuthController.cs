using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.ApiResponses;
using server.Data;
using server.Models;
using server.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext context;
        private IConfiguration _configuration;

        public AuthController(
            IConfiguration configuration, 
            DataContext _context)
        {
            _configuration = configuration;
            context = _context;
        }

        [Authorize]
        [HttpGet("/secret")]
        public IActionResult Secret()
        {
            LoginResponse response = new() { };

            response.type = "XD";
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] UserLogin userLogin, CancellationToken cancellationToken)
        {
            User? user = await Authenticate(userLogin);
            LoginResponse response = new() { };

            if (user is not null)
            {
                response.type = "success";
                string token = GenerateToken(user);
                response.error = token;
                Console.WriteLine(token);

                return Ok(response);
            }

            response.type = "error";
            response.error = "Invalid credentials.";
            return NotFound(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] UserRegister request, CancellationToken cancellationToken
        )
        {
            User? newUser = await this.context.Users.Where(u => u.Username == request.Username || u.Email == request.Email).FirstOrDefaultAsync();
            LoginResponse response = new() {};
            Console.WriteLine(request.Email + request.Password + request.Username);

            if (newUser is null)
            {
                var password = PasswordHasher.Hash(request.Password).Result;

                var registerUser = new User()
                {
                    Username = request.Username,
                    Password = password,
                    Email = request.Email,
                    RegisterDate = DateTime.UtcNow                    
                };

                context.Users.Add(registerUser);
                context.SaveChanges();

                response.type = "success";

                return Ok(response);
            }
            
            response.type = "error";
            response.error = "Username or/and email taken.";
            return BadRequest(response);
        }

        private async Task<User?> Authenticate(UserLogin userLogin)
        {
            User? possibleUser = await this.context.Users.Where(u => u.Username == userLogin.Username).FirstOrDefaultAsync();

            if (possibleUser != null && PasswordHasher.Verify(possibleUser.Password, userLogin.Password).Result)
            {
                return possibleUser;
            }

            return null;
        }

        private string GenerateToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               "Issuer",
               "Audience",
               claims,
               null,
               DateTime.UtcNow.AddDays(31),
               signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
