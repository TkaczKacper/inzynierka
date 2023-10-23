using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using server.Authorization;
using server.Models;
using server.Models.Profile;
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
            IActivityService activityService,
            IStravaService stravaService, 
            IJwtUtils jwtUtils)
        {
            _activityService = activityService;
            _stravaService = stravaService;
            _jwtUtils = jwtUtils;
        }

        [HttpPost("profile/update")]
        public async Task<IActionResult> ProfileUpdate([FromBody] StravaProfile stravaProfile)
        {
            var userID = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            string? stravaAccessToken = Request.Cookies["strava_access_token"];
            string? stravaRefreshToken = Request.Cookies["strava_refresh_token"];
            var response = await _stravaService.ProfileUpdate(stravaProfile, userID, stravaAccessToken, stravaRefreshToken);
            
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

            if (stravaAccessToken is null)
            {
                return BadRequest("Strava access token does not provided. Reload page or connect strava account and try again.");
            }

            var process = await _activityService.GetActivityDetails(stravaAccessToken, userID);
            
            return Ok($"Done. {process}");
        }

        [HttpPost("profile/hr-update")]
        public IActionResult HrUpdate([FromBody] ProfileHeartRate heartRate)
        {
            var userID = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            var response = _stravaService.ProfileHeartRateUpdate(heartRate, (Guid)userID);

            return Ok(response);
        }
        
        [HttpPost("profile/power-update")]
        public IActionResult PowerUpdate([FromBody] ProfilePower power)
        {
            var userID = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

            var response = _stravaService.ProfilePowerUpdate(power, (Guid)userID);

            return Ok(response);
        }

        [HttpGet("get-athlete-stats")]
        public IActionResult GetUserStats()
        {
            Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            var response =  _stravaService.GetProfileData(userId);
            
            return Ok(response);
        }
        
        [HttpGet("profile/get-activities")]
        public IActionResult GetUserActivities()
        {
            Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            var response = _stravaService.GetAthleteActivities(userId, null,null);
            Console.WriteLine(response);
            //if (response.IsNullOrEmpty()) return NotFound("Athlete does not have any activities yet.");
            return Ok(response);
        }

        [HttpGet("get-synced-activites")]
        public IActionResult GetSyncedActivitiesId()
        {
            Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            var syncedActivitiesId = _stravaService.GetSyncedActivitiesId(userId);
            var latestActivityDate = _stravaService.GetLatestActivity(userId);
            var response = new SyncedActivities()
            {
                SyncedActivitiesId = syncedActivitiesId,
                LatestActivityDateTime = latestActivityDate
            };
            
            return Ok(response);
        }

        [HttpGet("activities/get-activity-by-id/{activityId:long}")]
        public IActionResult GetActivityById(long activityId)
        {
            Console.WriteLine($"activity: {activityId}");
            var response = _activityService.GetActivityById(activityId);

            return Ok(response);
        }
    }
}
