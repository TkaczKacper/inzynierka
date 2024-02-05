using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Services;

namespace server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("process-activity/")]
    public class ProcessActivityController : ControllerBase
    {
        private IProcessActivityService _processService;
        private IJwtUtils _jwtUtils;
        
        public ProcessActivityController(
            IProcessActivityService processService,
            IJwtUtils jwtUtils)
        {
            _processService = processService;
            _jwtUtils = jwtUtils;
        }
        

        [HttpPost("get-activity-details")]
        public async Task<IActionResult> GetActivityDetailsAsync([FromBody] List<long> activityIds)
        {
            Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            string? stravaAccessToken = Request.Cookies["strava_access_token"];
            
            if (stravaAccessToken == null) return Unauthorized("Strava access token missing.");
            
            var response = await _processService.SaveActivitiesToFetch(activityIds, userId);

            return Ok(response);
        }
        

        [HttpGet("process-data")]
        public async Task<IActionResult> ProcessData()
        {
            Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
            string? stravaAccessToken = Request.Cookies["strava_access_token"];

            if (stravaAccessToken is null)
            {
                return BadRequest("Strava access token does not provided. Reload page or connect strava account and try again.");
            }

            var process = await _processService.GetActivityDetails(stravaAccessToken, userId);
            
            return Ok($"Done. {process}");
        }
    }
}
