using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Models;
using server.Models.Profile;
using server.Responses;
using server.Services;

namespace server.Controllers;

[Authorize]
[ApiController]
[Route("profile/")]
public class ProfileController : ControllerBase
{
    private IJwtUtils _jwtUtils;
    private IActivityService _activityService;
    private IStravaService _stravaService;

    public ProfileController(
        IJwtUtils jwtUtils,
        IActivityService activityService,
        IStravaService stravaService)
    {
        _jwtUtils = jwtUtils;
        _activityService = activityService;
        _stravaService = stravaService;
    }

    [HttpPost("update")]
    public async Task<IActionResult> ProfileUpdate([FromBody] StravaProfile stravaProfile)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        string? stravaAccessToken = Request.Cookies["strava_access_token"];
        string? stravaRefreshToken = Request.Cookies["strava_refresh_token"];
        
        var response = await _stravaService.ProfileUpdate(stravaProfile, userId, stravaAccessToken, stravaRefreshToken);
            
        return Ok(response);
    }
    
    
    [HttpPost("hr-update")]
    public IActionResult HrUpdate([FromBody] ProfileHeartRate heartRate)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response = _stravaService.ProfileHeartRateUpdate(heartRate, (Guid)userId);

        return Ok(response);
    }

    [HttpDelete("hr-delete-entry/{id:int}")]
    public IActionResult HrDeleteEntry(int id)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        var response = _stravaService.ProfileHeartRateDelete(id, userId);

        return Ok(response);
    }
    
    
    [HttpPost("power-update")]
    public IActionResult PowerUpdate([FromBody] ProfilePower power)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response = _stravaService.ProfilePowerUpdate(power, (Guid)userId);
    
        return Ok(response);
    }
    
    [HttpDelete("power-delete-entry/{id:int}")]
    public IActionResult PowerDeleteEntry(int id)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        var response = _stravaService.ProfilePowerDelete(id, userId);

        return Ok(response);
    }
    
    [HttpGet("get-activities")]
    public IActionResult GetUserActivities(DateTime? lastActivityDate, int? perPage)
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response = _stravaService.GetAthleteActivities(userId, lastActivityDate, perPage);
                
        return Ok(response);
    }
    
    
    [HttpGet("get-user-training-load")]
    public IActionResult GetUserTrainingLoadData()
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var trainingLoadData = _activityService.GetUserTrainingLoad(userId);
    
        return Ok(trainingLoadData);
    }
    
    
    [HttpGet("get-athlete-stats")]
    public IActionResult GetUserStats()
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response =  _stravaService.GetProfileData(userId);
        response.MonthlySummaries = _stravaService.GetMonthlyStats(userId);
        
        return Ok(response);
    }
    
    
    [HttpGet("get-synced-activites")]
    public IActionResult GetSyncedActivitiesId()
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var syncedActivitiesId = _stravaService.GetSyncedActivitiesId(userId);
        var latestActivityDate = _stravaService.GetLatestActivity(userId);
        var response = new SyncedActivities
        {
            SyncedActivitiesId = syncedActivitiesId,
            LatestActivityDateTime = latestActivityDate
        };
                
        return Ok(response);
    }
    

}