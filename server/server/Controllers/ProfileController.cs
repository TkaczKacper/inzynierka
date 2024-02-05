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
    private IProcessActivityService _processService;

    public ProfileController(
        IJwtUtils jwtUtils,
        IActivityService activityService,
        IStravaService stravaService,
        IProcessActivityService processService)
    {
        _jwtUtils = jwtUtils;
        _activityService = activityService;
        _stravaService = stravaService;
        _processService = processService;
    }

    [HttpPost("update")]
    public async Task<IActionResult> ProfileUpdate([FromBody] StravaProfile stravaProfile)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        string? stravaRefreshToken = Request.Cookies["strava_refresh_token"];
        
        var response = await _stravaService.ProfileUpdate(stravaProfile, userId, stravaRefreshToken);
            
        return Ok(response);
    }
    
    
    [HttpPost("hr-update")]
    public async Task<IActionResult> HrUpdate([FromBody] ProfileHeartRate heartRate)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response = await _stravaService.ProfileHeartRateUpdate(heartRate, (Guid)userId);

        return Ok(response);
    }

    [HttpDelete("hr-delete-entry/{id:int}")]
    public async Task<IActionResult> HrDeleteEntry(int id)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        var response = await _stravaService.ProfileHeartRateDelete(id, userId);

        return Ok(response);
    }
    
    
    //TODO dodac obsluge ustawiania stref
    [HttpPost("power-update")]
    public async Task<IActionResult> PowerUpdate([FromBody] ProfilePower power)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response = await _stravaService.ProfilePowerUpdate(power, (Guid)userId);
    
        return Ok(response);
    }
    
    [HttpDelete("power-delete-entry/{id:int}")]
    public async  Task<IActionResult> PowerDeleteEntry(int id)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        var response = await _stravaService.ProfilePowerDelete(id, userId);

        return Ok(response);
    }
    
    [HttpGet("get-activities")]
    public IActionResult GetUserActivities(DateTime? lastActivityDate, int? perPage)
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var last = DateTime.UtcNow;
        if (lastActivityDate != null)
        {
            last = DateTime.Parse(lastActivityDate.ToString()).ToUniversalTime();  
        }
        Console.WriteLine(last);
        var response = _stravaService.GetAthleteActivities(userId, last, perPage);
                
        return Ok(response);
    }

    [HttpGet("get-activities-period")]
    //TODO domyslnie ostatnie 12mies
    public IActionResult GetUserPeriodActivities(int? month, int? yearOffset)
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response = _stravaService.GetAthletePeriodActivities(userId, month, yearOffset);

        return Ok(response);
    }
    
    
    [HttpGet("get-user-training-load")]
    public async Task<IActionResult> GetUserTrainingLoadData()
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var trainingLoadData = await _processService.GetUserTrainingLoad(userId);
    
        return Ok(trainingLoadData);
    }
    
    
    [HttpGet("get-athlete-stats")]
    public IActionResult GetUserStats(int yearOffset)
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        var response =  _stravaService.GetProfileData(userId);
        response.MonthlySummaries = _stravaService.GetMonthlyStats(userId, yearOffset);
        
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