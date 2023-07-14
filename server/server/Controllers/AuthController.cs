using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.ApiResponses;
using server.Data;
using server.Models;
using server.Utilities;

namespace server.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext context;
        private IConfiguration _configuration;

        public AuthController(IConfiguration configuration, DataContext _context)
        {
            _configuration = configuration;
            context = _context;
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
    }
}
