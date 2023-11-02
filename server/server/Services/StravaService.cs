using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Helpers;
using server.Models;
using server.Models.Profile;
using server.Models.Strava;
using server.Responses;

namespace server.Services
{
    public interface IStravaService
    {
        Task<StravaProfile> ProfileUpdate(StravaProfile profileInfo, Guid? userId, string? accesstoken, string? refreshtoken);
        Task<string> SaveActivitiesToFetch(List<long> activityIds, Guid? userId);
        ProfileHeartRate ProfileHeartRateUpdate(ProfileHeartRate profileHeartRate, Guid userId);
        ProfilePower ProfilePowerUpdate(ProfilePower profilePower, Guid userId);
        AthleteData GetProfileData(Guid? userId);
        IEnumerable<StravaActivity> GetAthleteActivities(Guid? userId, DateTime? lastActivityDate, int? perPage);
        List<long> GetSyncedActivitiesId(Guid? userId);
        DateTime GetLatestActivity(Guid? userId);
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
        private static HttpClient stravaClient = new()
        {
            BaseAddress = new Uri("https://www.strava.com/api/v3/"),
        };

        public async Task<StravaProfile> ProfileUpdate(StravaProfile profile, Guid? id, string? accesstoken, string? refreshtoken)
        {
            Console.WriteLine("profile update");
            User? user = GetUserById(id);
            stravaClient.DefaultRequestHeaders.Clear();
            stravaClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accesstoken}");

            var athleteStats = await _stravaApi.GetAthleteStats(profile.ProfileID, stravaClient);           
            
            StravaProfile profileDetails = new StravaProfile
            {
                StravaRefreshToken = refreshtoken, 
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
                ProfileCreatedAt = profile.ProfileCreatedAt,
                NeedUpdate = false,
            };
            user.StravaProfile = profileDetails;
            user.StravaProfile.AthleteStats = athleteStats;
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

            user.ActivitiesToFetch = activitiesToSync;
            _context.Update(user);
            _context.SaveChanges();
            Console.WriteLine($"profile: {user.StravaProfile.LastName}");
           

            return $"Remaining activities to be fetch: {activitiesToSync.Count}";
        }

        public ProfileHeartRate ProfileHeartRateUpdate(ProfileHeartRate profileHeartRate, Guid userId)
        {
            int hrMax = (int)profileHeartRate.HrMax;

            ProfileHeartRate userHr = new ProfileHeartRate
            {
                DateAdded = DateOnly.FromDateTime(DateTime.UtcNow),
                HrRest = profileHeartRate.HrRest,
                HrMax = hrMax,
                
                Zone1 = (int)Math.Round(0.5 * hrMax),
                Zone2 = (int)Math.Round(0.6 * hrMax),
                Zone3 = (int)Math.Round(0.7 * hrMax),
                Zone4 = (int)Math.Round(0.8 * hrMax),
                Zone5 = (int)Math.Round(0.9 * hrMax),
                
                UserID = userId
            };
            _context.ProfileHeartRate.Add(userHr);
            _context.SaveChanges();

            return userHr;
        }
        
        public ProfilePower ProfilePowerUpdate(ProfilePower profilePower, Guid userId)
        {
            int ftp = (int)profilePower.FTP;
            ProfilePower power = new ProfilePower
            {
                DateAdded = DateOnly.FromDateTime(DateTime.UtcNow),
                FTP = ftp,
                UserID = userId,
                Zone1 = 0,
                Zone2 = (int)Math.Floor(0.55 * ftp),
                Zone3 = (int)Math.Floor(0.75 * ftp),
                Zone4 = (int)Math.Floor(0.90 * ftp),
                Zone5 = (int)Math.Floor(1.05 * ftp),
                Zone6 = (int)Math.Floor(1.20 * ftp),
                Zone7 = (int)Math.Floor(1.75 * ftp)
            };

            _context.ProfilePower.Add(power);
            _context.SaveChanges();

            return power;
        }

        public AthleteData GetProfileData(Guid? userId)
        {
            StravaProfile? profile = GetUserById(userId).StravaProfile;
            if (profile is null) return null;
            StravaProfileStats? stats = GetAthleteStats(profile.AthleteStatsId);

            List<ProfileHeartRate>? heartRate = _context.ProfileHeartRate
                .Where(phr => phr.UserID == userId)
                .ToList();

            List<ProfilePower>? power = _context.ProfilePower
                .Where(pp => pp.UserID == userId)
                .ToList();
            
            AthleteData response = new AthleteData
            {
                AthleteStats = stats,
                StravaProfileInfo = profile,
                HrZones = heartRate,
                PowerZones = power
            };
            
            return response;
        }
        public IEnumerable<StravaActivity> GetAthleteActivities(Guid? userId, DateTime? lastActivityDate, int? perPage)
        {
            perPage ??= 10;
            lastActivityDate ??= DateTime.UtcNow;
            var activities = GetSyncedActivities((Guid)userId, (DateTime)lastActivityDate, (int)perPage);

            return activities;
        }

        public DateTime GetLatestActivity(Guid? userId)
        {
            var date = _context.StravaActivity
                .Where(sa => sa.UserId == userId)
                .OrderByDescending(sa => sa.StartDate)
                .Select(sa => sa.StartDate)
                .FirstOrDefault();
            return date;
        }
        
        // helpers 
        public User GetUserById(Guid? id)
        {
            User? user = _context.Users
                .Include(u => u.StravaProfile)
                .FirstOrDefault(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }

        public StravaProfileStats? GetAthleteStats(long id)
        {
            StravaProfileStats? profile = _context.StravaProfileStats
                .Include(stats => stats.RecentRideTotals)
                .Include(stats => stats.YtdRideTotals)
                .Include(stats => stats.AllTimeRideTotals)
                .FirstOrDefault(s => s.Id == id);
            
            return profile == null ? throw new KeyNotFoundException("User not found.") : profile;
        }
        
        public List<long> GetSyncedActivitiesId(Guid? id)
        {

            List<long>? userActivitiesId = _context.StravaActivity
                .Where(sa => sa.UserId == id)
                .Select(sa => sa.StravaActivityID)
                .ToList();

            return userActivitiesId;
        }

        public List<StravaActivity> GetSyncedActivities(Guid userId, DateTime date, int perPage)
        {
            var activities = _context.StravaActivity
                .Where(a => a.UserId == userId && a.StartDate < date) 
                .Include(a=> a.Laps)
                .OrderByDescending(a=> a.StartDate)
                .Take(perPage)
                .ToList();

            return activities;
        }
    }
}
