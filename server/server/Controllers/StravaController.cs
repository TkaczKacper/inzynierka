using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Responses;
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
            IStravaService stravaService, 
            IJwtUtils jwtUtils)
        {
            _stravaService = stravaService;
            _jwtUtils = jwtUtils;
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

            if (stravaAccessToken is null)
            {
                return BadRequest("Strava access token does not provided. Reload page or connect strava account and try again.");
            }

            var process = await _activityService.GetActivityDetails(stravaAccessToken, userID);
            
            return Ok($"Done. {process}");
        }
    }
}
