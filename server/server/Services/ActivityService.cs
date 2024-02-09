using System.Globalization;
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
                .Include(x => x.PowerTimeInZone)
                .Include(x => x.HrTimeInZone)
                .SingleOrDefaultAsync(sa => sa.ID == activityId);
            
            return activity == null ? throw new KeyNotFoundException("Activity not found.") : activity;
        }

        public async Task<string> DeleteActivityById(long activityId, Guid userId)
        {
            var activity = await GetActivityById(activityId);
            
            _context.RemoveRange(_context.ActivityLap.Where(lap => lap.ActivityId == activityId));
            _context.Remove(activity.PowerTimeInZone);
            _context.Remove(activity.HrTimeInZone);
            _context.Remove(activity);
            await _context.ProfileYearlySummary
                .Where(x => x.Year == activity.StartDate.Year)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.TotalDistance, x => x.TotalDistance - activity.TotalDistance)
                        .SetProperty(x => x.TotalCalories, x => x.TotalCalories - activity.Calories)
                        .SetProperty(x => x.TrainingLoad, x => x.TrainingLoad - activity.TrainingLoad)
                        .SetProperty(x => x.TotalElevationGain, x => x.TotalElevationGain - activity.TotalElevationGain)
                        .SetProperty(x => x.TotalElapsedTime, x => x.TotalElapsedTime - activity.ElapsedTime)
                        .SetProperty(x => x.TotalMovingTime, x => x.TotalMovingTime - activity.MovingTime)
                );
            
            await _context.ProfileMonthlySummary
                .Where(x => x.Year == activity.StartDate.Year && x.Month == activity.StartDate.Month)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.TotalDistance, x => x.TotalDistance - activity.TotalDistance)
                        .SetProperty(x => x.TotalCalories, x => x.TotalCalories - activity.Calories)
                        .SetProperty(x => x.TrainingLoad, x => x.TrainingLoad - activity.TrainingLoad)
                        .SetProperty(x => x.TotalElevationGain, x => x.TotalElevationGain - activity.TotalElevationGain)
                        .SetProperty(x => x.TotalElapsedTime, x => x.TotalElapsedTime - activity.ElapsedTime)
                        .SetProperty(x => x.TotalMovingTime, x => x.TotalMovingTime - activity.MovingTime)
                );
            
            CultureInfo myCi = new CultureInfo("pl-PL");
            Calendar myCal = myCi.Calendar;
            int weekNumber = myCal.GetWeekOfYear(activity.StartDate, CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
            
            await _context.ProfileWeeklySummary
                .Where(x => x.Year == activity.StartDate.Year && x.Week == weekNumber)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.TotalDistance, x => x.TotalDistance - activity.TotalDistance)
                        .SetProperty(x => x.TotalCalories, x => x.TotalCalories - activity.Calories)
                        .SetProperty(x => x.TrainingLoad, x => x.TrainingLoad - activity.TrainingLoad)
                        .SetProperty(x => x.TotalElevationGain, x => x.TotalElevationGain - activity.TotalElevationGain)
                        .SetProperty(x => x.TotalElapsedTime, x => x.TotalElapsedTime - activity.ElapsedTime)
                        .SetProperty(x => x.TotalMovingTime, x => x.TotalMovingTime - activity.MovingTime)
                );

            await _context.TrainingLoads
                .Where(x => x.Date == DateOnly.FromDateTime(activity.StartDate))
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.TrainingLoad, 0)
                        .SetProperty(x => x.TrainingImpulse, 0)
                );
            
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
