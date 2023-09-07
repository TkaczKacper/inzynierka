using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Models;
using server.Services;

namespace server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("strava/")]
    public class StravaController : ControllerBase
    {
        private IStravaService _stravaService;
        private IJwtUtils _jwtUtils;
        
        public StravaController(IStravaService stravaService, IJwtUtils jwtUtils)
        {
            _stravaService = stravaService;
            _jwtUtils = jwtUtils;
        }

        [HttpPost("profile/update")]
        public IActionResult ProfileUpdate([FromBody] StravaProfile stravaProfile)
        {
            var userID = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

            var response = _stravaService.ProfileUpdate(stravaProfile, userID);
            
            return Ok(response);
        }
    }
}
