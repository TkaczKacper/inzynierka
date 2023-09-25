using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Helpers;
using server.Models;
using server.Models.Strava;

namespace server.Services
{
    public interface IStravaService
    {
        StravaProfile ProfileUpdate(StravaProfile profileInfo, Guid? userId);
        Task<string> SaveActivitiesToFetch(List<long> activityIds, Guid? userId);
    }

    public class StravaService : IStravaService
    {
        private readonly StravaSettings _stravaSettings;
        private readonly DataContext _context;
        private IStravaApiService _stravaApi;

        public StravaService(
            IOptions<StravaSettings> stravaSettings,
            DataContext context,
            IStravaApiService stravaApi)
        {
            _stravaSettings = stravaSettings.Value;
            _context = context;
            _stravaApi = stravaApi;
        }

        public StravaProfile ProfileUpdate(StravaProfile profile, Guid? id)
        {
            Console.WriteLine("profile update");
            User? user = GetUserById(id);

            StravaProfile profileDetails = new StravaProfile
            {
                StravaRefreshToken = profile.StravaRefreshToken,
                ProfileID = profile.ProfileID,
                Username = profile.Username,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Sex = profile.Sex,
                Bio = profile.Bio,
                ProfileAvatar = profile.ProfileAvatar,
                Country = profile.Country,
                State = profile.State,
                City = profile.City,
                Weight = profile.Weight,
                ProfileCreatedAt = profile.ProfileCreatedAt
            };

            Console.WriteLine(profileDetails);

            user.StravaProfile = profileDetails;

            _context.Update(user);
            _context.SaveChanges();

            return profile;
        }

        public async Task<string> SaveActivitiesToFetch(List<long> activities, Guid? userId)
        {
            Console.WriteLine("Saving...");
            
            User user = GetUserById(userId);
            List<long> syncedActivities = GetSyncedActivitiesId(userId);

            List<long> activitiesToSync = activities.Where(id => !syncedActivities.Contains(id)).ToList();

            user.StravaProfile.ActivitiesToFetch = activitiesToSync;
            _context.Update(user);
            _context.SaveChanges();
            Console.WriteLine($"profile: {user.StravaProfile.LastName}");
           

            return $"Remaining activities to be fetch: {activitiesToSync.Count}";
        }


        // helpers 
        public User GetUserById(Guid? id)
        {
            User? user = _context.Users.Include(u => u.StravaProfile).Include(u => u.StravaProfile.Activities).FirstOrDefault(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }

        public List<long> GetSyncedActivitiesId(Guid? id)
        {
            List<long>? userActivitiesId = _context.Users
                .Where(u => u.ID == id)
                .Select(u => u.StravaProfile)
                .SelectMany(s => s.Activities)
                .Select(a => a.StravaActivityID)
                .ToList();

            return userActivitiesId;
        }
    }
}
