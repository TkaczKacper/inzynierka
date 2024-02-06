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
        private readonly IUserService _userService;
        private readonly IJwtUtils _jwtUtils;

        public AuthController(IUserService userService, IJwtUtils jwtUtils)
        {
            _userService = userService;
            _jwtUtils = jwtUtils;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest userLogin, CancellationToken cancellationToken)
        {
            var response = await _userService.Authenticate(userLogin, IpAddress());
            SetTokenCookie(response.RefreshToken);
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
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

            if (userId is null) return Unauthorized();
            
            var response = await _userService.ChangePassword((Guid)userId, changePassword);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest userRegister, CancellationToken cancellation) 
        {
            var response = await _userService.Register(userRegister, IpAddress());
            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout() 
        {
            var token = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required." });

            await _userService.RevokeToken(token, IpAddress());
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("jwtToken");
            Response.Cookies.Delete("strava_access_token");
            Response.Cookies.Delete("strava_refresh_token");
            
            return Ok(new { message = "Logged out."});
        }
        
        
        [AllowAnonymous]
        [HttpGet("renew-token")]
        public async Task<IActionResult> RenewAccessToken(){
            string? token = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token)) {
                return BadRequest(new { message = "Token is required." });
            }
            var response = await _userService.RenewAccessToken(token);
            SetTokenCookie(response.RefreshToken);
            
            return Ok(response);
            
        }
        
        //TODO dodac usuwanie konta
        

        // helper methods
        private void SetTokenCookie(string token)
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

        private string IpAddress()
        {
            // get source ip address for request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded_For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
