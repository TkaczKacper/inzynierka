using Microsoft.EntityFrameworkCore;
using server.Helpers;
using server.Models;
using server.Models.Activity;

namespace server.Services
{
    public interface IActivityService
    {
        Task<Activity> GetActivityById(long activityId);
        Task<string>  DeleteActivityById(long activityId, Guid userId);
    }

    public class ActivityService : IActivityService
    {
        private readonly DataContext _context;
        private readonly IProcessActivityService _processService;

        public ActivityService(DataContext context, IProcessActivityService processService)
        {
            _context = context;
            _processService = processService;
        }

        public async Task<Activity> GetActivityById(long activityId)
        {
            var activity = await _context.Activity
                .Include(a => a.HrTimeInZone)
                .Include(a => a.PowerTimeInZone)
                .SingleOrDefaultAsync(sa => sa.ID == activityId);
            
            return activity == null ? throw new KeyNotFoundException("Activity not found.") : activity;
        }

        public async Task<string> DeleteActivityById(long activityId, Guid userId)
        {
            var activity = await GetActivityById(activityId);
            
            _context.RemoveRange(_context.ActivityLap.Where(lap => lap.ActivityId == activityId));
            _context.Remove(activity);
            
            await _context.SaveChangesAsync();

            await _processService.UpdateTrainingLoad(userId);
            
            return "Deleted";
        }
        
        
        public async Task<User> GetUserByIdAsync(Guid? id)
        {
            User? user = await _context.Users.Include(u => u.StravaProfile).Include(u => u.UserHeartRate)
                .Include(u => u.UserPower).FirstOrDefaultAsync(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
    }
}
