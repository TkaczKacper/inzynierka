using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Models;
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

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] AuthRequest userLogin, CancellationToken cancellationToken)
        {
            var response = _userService.Authenticate(userLogin, ipAddress());
            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("logout")]
        public IActionResult Logout(RevokeTokenRequest model) 
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required." });

            _userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Logged out."});
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userService.GetById(id);

            return Ok(user.RefreshTokens);
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
                Expires = DateTime.UtcNow.AddMinutes(30)
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
