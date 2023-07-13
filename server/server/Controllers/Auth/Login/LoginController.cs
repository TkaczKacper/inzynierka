using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Models;

namespace server.Controllers.Auth.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DataContext context;
        private IConfiguration _configuration;

        public LoginController(IConfiguration configuration, DataContext _context)
        {
            _configuration = configuration;
            this.context = _context;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin user)
        {

            return null;
        }
    }
}
