using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Models.Authenticate;
using server.Services;

namespace server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/auth/")]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IJwtUtils _jwtUtils;

        public AuthController(IUserService userService, IJwtUtils jwtUtils)
        {
            _userService = userService;
            _jwtUtils = jwtUtils;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] LoginRequest userLogin, CancellationToken cancellationToken)
        {
            var response = _userService.Authenticate(userLogin, ipAddress());
            setTokenCookie(response.RefreshToken);
            try
            {
                Response.Cookies.Append("strava_refresh_token", response.StravaRefreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return Ok(response);
        }

        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePassword changePassword)
        {
            var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            
            var response = _userService.ChangePassword(userId, changePassword);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest userRegister, CancellationToken cancellation) 
        {
            var response = _userService.Register(userRegister, ipAddress());
            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        
        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult Logout() 
        {
            var token = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required." });

            _userService.RevokeToken(token, ipAddress());
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("jwtToken");
            Response.Cookies.Delete("strava_access_token");
            Response.Cookies.Delete("strava_refresh_token");
            return Ok(new { message = "Logged out."});
        }
        
        
        [AllowAnonymous]
        [HttpGet("renew-token")]
        public IActionResult RenewAccessToken(){
            string? token = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token)) {
                return BadRequest(new { message = "Token is required." });
            }
            var response = _userService.RenewAccessToken(token);
            setTokenCookie(response.RefreshToken);
            return Ok(response);
            
        }
        

        // helper methods
        private void setTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(31)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            // get source ip address for request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded_For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
