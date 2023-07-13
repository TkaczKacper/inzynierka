using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.ApiResponses;
using server.Data;
using server.Models;
using server.Utilities;

namespace server.Controllers
{
    [Route("api/[controller]")]
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

        [AllowAnonymous]
        [HttpPost]
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
