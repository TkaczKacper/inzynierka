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
    private IStravaService _stravaService;
    private readonly IHelperService _helperService;
    private IProcessActivityService _processService;

    public ProfileController(
        IJwtUtils jwtUtils,
        IStravaService stravaService,
        IHelperService helperService,
        IProcessActivityService processService)
    {
        _jwtUtils = jwtUtils;
        _stravaService = stravaService;
        _helperService = helperService;
        _processService = processService;
    }

    [HttpPost("update")]
    public async Task<IActionResult> ProfileUpdate([FromBody] StravaProfile stravaProfile)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        
        
        if (userId is null) return Unauthorized();
        
        string? stravaRefreshToken = Request.Cookies["strava_refresh_token"];
        if (stravaRefreshToken is null) return BadRequest("Something went wrong with that request, try again later.");
        
        var response = await _stravaService.ProfileUpdate(stravaProfile, (Guid)userId, stravaRefreshToken);
            
        return Ok(response);
    }
    
    
    [HttpPost("hr-update")]
    public async Task<IActionResult> HrUpdate([FromBody] ProfileHeartRate heartRate)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        if (userId is null) return Unauthorized();
        
        var response = await _stravaService.ProfileHeartRateUpdate(heartRate, (Guid)userId);

        return Ok(response);
    }

    [HttpDelete("hr-delete-entry/{id:int}")]
    public async Task<IActionResult> HrDeleteEntry(int id)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        if (userId is null) return Unauthorized();
        
        var response = await _stravaService.ProfileHeartRateDelete(id, (Guid)userId);

        return Ok(response);
    }
    
    
    //TODO dodac obsluge ustawiania stref
    [HttpPost("power-update")]
    public async Task<IActionResult> PowerUpdate([FromBody] ProfilePower power)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        
        if (userId is null) return Unauthorized();
        
        var response = await _stravaService.ProfilePowerUpdate(power, (Guid)userId);
    
        return Ok(response);
    }
    
    [HttpDelete("power-delete-entry/{id:int}")]
    public async  Task<IActionResult> PowerDeleteEntry(int id)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        
        if (userId is null) return Unauthorized();

        var response = await _stravaService.ProfilePowerDelete(id, (Guid)userId);

        return Ok(response);
    }
    
    [HttpGet("get-activities")]
    public async Task<IActionResult> GetUserActivities(DateTime? lastActivityDate, int? perPage)
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        
        if (userId is null) return Unauthorized();

        perPage ??= 10;
        DateTime last = DateTime.UtcNow;
        if (lastActivityDate != null)
        {
            last = DateTime.Parse(lastActivityDate.ToString()).ToUniversalTime();  
        }
        
        var response = await _helperService.GetAthleteActivities((Guid)userId, last, (int)perPage);
                
        return Ok(response);
    }

    [HttpGet("get-activities-period")]
    //TODO domyslnie ostatnie 12mies
    public async Task<IActionResult> GetUserPeriodActivities(int? month, int? yearOffset)
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        
        if (userId is null) return Unauthorized();
        
        var response = await _stravaService.GetAthletePeriodActivities((Guid)userId, month, yearOffset);

        return Ok(response);
    }
    
    
    [HttpGet("get-user-training-load")]
    public async Task<IActionResult> GetUserTrainingLoadData()
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        
        if (userId is null) return Unauthorized();
        
        var trainingLoadData = await _processService.GetUserTrainingLoad((Guid)userId);
    
        return Ok(trainingLoadData);
    }
    
    
    [HttpGet("get-athlete-monthly-stats")]
    public async Task<IActionResult> GetUserStats(int yearOffset)
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        if (userId is null) return Unauthorized();
        
        var response =  await _stravaService.GetProfileData((Guid)userId);
        response.MonthlySummaries = await _helperService.GetAthleteMonthlySummary((Guid)userId, yearOffset);
        
        return Ok(response);
    }
    
    
    [HttpGet("get-synced-activities")]
    public async Task<IActionResult> GetSyncedActivitiesId()
    {
        Guid? userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);
        
        if (userId is null) return Unauthorized();
        
        var syncedActivitiesId = await _helperService.GetSyncedActivitiesId((Guid)userId);
        var latestActivityDate = await _helperService.GetLatestActivity((Guid)userId);
        var response = new SyncedActivities
        {
            SyncedActivitiesId = syncedActivitiesId,
            LatestActivityDateTime = latestActivityDate
        };
                
        return Ok(response);
    }
}