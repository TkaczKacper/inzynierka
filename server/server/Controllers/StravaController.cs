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
        private IActivityService _activityService;
        private IStravaService _stravaService;
        private IJwtUtils _jwtUtils;
        
        public StravaController(
            IActivityService activityService,
            IStravaService stravaService, 
            IJwtUtils jwtUtils)
        {
            _activityService = activityService;
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
        public async Task<IActionResult> GetActivityDetailsAsync([FromBody] List<long> activityIds)
        {
            Guid? userID = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            string? stravaAccessToken = Request.Cookies["strava_access_token"];
            if (stravaAccessToken != null)
            {
                var response = await _stravaService.SaveActivitiesToFetch(activityIds, userID);

                return Ok(response);
            }
            return Unauthorized("Strava access token missing.");
        }

        [HttpGet("process-data")]
        public async Task<IActionResult> ProcessData()
        {
            Guid? userID = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            string? stravaAccessToken = Request.Cookies["strava_access_token"];

            var process = await _activityService.GetActivityDetails(stravaAccessToken, userID);
            return Ok("Done.");
        }
    }
}
