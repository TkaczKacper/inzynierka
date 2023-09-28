using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Helpers;
using server.Models;
using server.Models.Profile;
using server.Models.Strava;

namespace server.Services
{
    public interface IStravaService
    {
        StravaProfile ProfileUpdate(StravaProfile profileInfo, Guid? userId);
        Task<string> SaveActivitiesToFetch(List<long> activityIds, Guid? userId);
        ProfileHeartRate ProfileHeartRateUpdate(int hrRest, int hrMax, Guid? userId);
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
            List<long> syncedActivities = GetSyncedActivitiesId(user.StravaProfile.ID);

            List<long> activitiesToSync = activities.Where(id => !syncedActivities.Contains(id)).ToList();

            user.StravaProfile.ActivitiesToFetch = activitiesToSync;
            _context.Update(user);
            _context.SaveChanges();
            Console.WriteLine($"profile: {user.StravaProfile.LastName}");
           

            return $"Remaining activities to be fetch: {activitiesToSync.Count}";
        }

        public ProfileHeartRate ProfileHeartRateUpdate(int hrRest, int hrMax, Guid? id)
        {
            User? user = GetUserHr(id);

            HrZones userHrZones = new HrZones
            {
                Zone1 = (int)Math.Round(0.5 * hrMax),
                Zone2 = (int)Math.Round(0.6 * hrMax),
                Zone3 = (int)Math.Round(0.7 * hrMax),
                Zone4 = (int)Math.Round(0.8 * hrMax),
                Zone5 = (int)Math.Round(0.9 * hrMax)
            };

            ProfileHeartRate userHr = new ProfileHeartRate
            {
                DateAdded = DateOnly.FromDateTime(DateTime.UtcNow),
                HrRest = hrRest,
                HrMax = hrMax,
                HrZones = userHrZones
            };

            user.UserHeartRate.Add(userHr);

            return userHr;
        }
        // helpers 
        public User GetUserById(Guid? id)
        {
            User? user = _context.Users
                .Include(u => u.StravaProfile)
                .FirstOrDefault(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }

        public List<long> GetSyncedActivitiesId(long? id)
        {

            List<long>? userActivitiesId = _context.StravaActivity
                .Where(sa => sa.StravaProfileID == id)
                .Select(sa => sa.StravaActivityID)
                .ToList();

            return userActivitiesId;
        }
        public User GetUserHr(Guid? id)
        {
            User? user = _context.Users
                .Include(u => u.UserHeartRate)
                .FirstOrDefault(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
        public User GetUserPower(Guid? id)
        {
            User? user = _context.Users
                .Include(u => u.UserPower)
                .FirstOrDefault(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
    }
}
