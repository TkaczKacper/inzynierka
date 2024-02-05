using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.Services;

namespace server.Controllers;

[Authorize]
[ApiController]
[Route("activity/")]
public class ActivityController : ControllerBase
{
    private IJwtUtils _jwtUtils;
    private IActivityService _activityService;

    public ActivityController(
        IJwtUtils jwtUtils,
        IActivityService activityService)
    {
        _jwtUtils = jwtUtils;
        _activityService = activityService;
    }
    
    [HttpGet("get-activity-by-id/{activityId:long}")]
    public async Task<IActionResult> GetActivityById(long activityId)
    {
        var response = await _activityService.GetActivityById(activityId);
    
        return Ok(response);
    }

    [HttpDelete("delete-activity-by-id/{activityId:long}")]
    public async Task<IActionResult> DeleteActivityById(long activityId)
    {
        var userId = _jwtUtils.ValidateJwtToken(Request.Headers.Authorization);

        var response = await _activityService.DeleteActivityById(activityId, userId);

        return Ok(response);
    }
    
}