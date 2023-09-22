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

        [HttpPost("get-activity-details")]
        public async Task<IActionResult> GetActivityDetails([FromBody] List<long> activityIds)
        {
            Guid? userID = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            string? stravaAccessToken = Request.Cookies["strava_access_token"];
            if (stravaAccessToken != null)
            {
                var response = _stravaService.SaveActivitiesToFetch(activityIds, userID);
                await _stravaService.GetActivityDetails(stravaAccessToken, userID);


                return Ok(response);
            }
            return Unauthorized("Strava access token missing.");
        }
    }
}
