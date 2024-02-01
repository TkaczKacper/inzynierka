using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Helpers;
using server.Models;
using server.Models.Profile;
using server.Models.Profile.Summary;
using server.Models.Activity;
using server.Responses;

namespace server.Services
{
        //TODO zmiana nazwy pliku bo nie obsluguje tego co wynika z nazwy
    public interface IStravaService
    {
        //TODO przeniesc do innego pliku 5 kolejnych
        Task<StravaProfile> ProfileUpdate(StravaProfile profileInfo, Guid? userId, string? refreshToken);
        ProfileHeartRate ProfileHeartRateUpdate(ProfileHeartRate profileHeartRate, Guid userId);
        string ProfileHeartRateDelete(long entryId, Guid? userId);
        ProfilePower ProfilePowerUpdate(ProfilePower profilePower, Guid userId);
        string ProfilePowerDelete(long entryId, Guid? userId);
        AthleteData GetProfileData(Guid? userId);
        
        //TODO dodac serwis do obslugi danych uzytkownika
        Task<string> SaveActivitiesToFetch(List<long> activityIds, Guid? userId);
        List<ProfileMonthlySummary> GetMonthlyStats(Guid? userId, int yearOffset);
        IEnumerable<Activity> GetAthleteActivities(Guid? userId, DateTime? lastActivityDate, int? perPage);
        IEnumerable<Activity> GetAthletePeriodActivities(Guid? userId, int? month, int? yearOffset);
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

        public async Task<StravaProfile> ProfileUpdate(StravaProfile profile, Guid? id, string? refreshToken)
        {
            Console.WriteLine("profile update");
            User? user = GetUserById(id);
            
            StravaProfile profileDetails = new StravaProfile
            {
                StravaRefreshToken = refreshToken, 
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
            };
            user.StravaProfile = profileDetails;
            
            _context.Update(user);
            await _context.SaveChangesAsync();

            return profile;
        }

        //TODO poprawic funkcje
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

        //TODO zmienic na async
        public ProfileHeartRate ProfileHeartRateUpdate(ProfileHeartRate profileHeartRate, Guid userId)
        {
            int? hrMax = profileHeartRate.HrMax;
            int? hrRest = profileHeartRate.HrRest;
            int? ltHr= profileHeartRate.LTHr;
            bool autoZones = profileHeartRate.SetAutoZones;

            ProfileHeartRate userHr = new ProfileHeartRate
            {
                UserID = userId
            };
            
            if (autoZones && hrMax != null && hrRest != null)
            {
                userHr.DateAdded = DateOnly.FromDateTime(DateTime.UtcNow);
                userHr.HrRest = profileHeartRate.HrRest;
                userHr.HrMax = hrMax;
                userHr.LTHr = ltHr;
                userHr.Zone1 = (int)hrRest;
                if (ltHr != null)
                {
                    userHr.Zone2 = (int)(0.81 * ltHr);
                    userHr.Zone3 = (int)(0.90 * ltHr);
                    userHr.Zone4 = (int)(0.94 * ltHr);
                    userHr.Zone5a = ltHr;
                    userHr.Zone5b = (int)(1.03 * ltHr);
                    userHr.Zone5c = (int)(1.06 * ltHr);
                }
                else
                {
                    userHr.Zone2 = (int)(0.65 * hrMax);
                    userHr.Zone3 = (int)(0.77 * hrMax);
                    userHr.Zone4 = (int)(0.86 * hrMax);
                    userHr.Zone5 = (int)(0.935 * hrMax);
                }
                _context.ProfileHeartRate.Add(userHr);
            }
            else
            {
                userHr.DateAdded = profileHeartRate.DateAdded;
                userHr.LTHr = profileHeartRate.LTHr;
                userHr.Zone1 = profileHeartRate.Zone1;
                userHr.Zone2 = profileHeartRate.Zone2;
                userHr.Zone3 = profileHeartRate.Zone3;
                userHr.Zone4 = profileHeartRate.Zone4;
                userHr.Zone5 = profileHeartRate.Zone5;
                userHr.Zone5a = profileHeartRate.Zone5a;
                userHr.Zone5b = profileHeartRate.Zone5b;
                userHr.Zone5c = profileHeartRate.Zone5c;
                
                _context.ProfileHeartRate
                    .Where(e => e.DateAdded == profileHeartRate.DateAdded)
                    .ExecuteUpdate(hr => hr
                        .SetProperty(x => x.Zone1, profileHeartRate.Zone1)
                        .SetProperty(x => x.Zone2, profileHeartRate.Zone2)
                        .SetProperty(x => x.Zone3, profileHeartRate.Zone3)
                        .SetProperty(x => x.Zone4, profileHeartRate.Zone4)
                        .SetProperty(x => x.Zone5, profileHeartRate.Zone5)
                        .SetProperty(x => x.Zone5a, profileHeartRate.Zone5a)
                        .SetProperty(x => x.Zone5b, profileHeartRate.Zone5b)
                        .SetProperty(x => x.Zone5c, profileHeartRate.Zone5c)
                        .SetProperty(x => x.LTHr, ltHr)
                    );
            }
            
            _context.SaveChanges();

            return userHr;
        }

        //TODO zmienic na async
        public string ProfileHeartRateDelete(long entryId, Guid? userId)
        {
            ProfileHeartRate? hrEntry = _context.ProfileHeartRate
                .FirstOrDefault(hr => hr.ID == entryId && hr.UserID == userId);

            if (hrEntry is null)
            {
                throw new AppException("Entry not found.");
            }
            
            _context.ProfileHeartRate.Remove(hrEntry);
            _context.SaveChanges();

            return "Deleted";
        } 
        
        //TODO zmienic na async
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
        
        //TODO zmienic na async
        public string ProfilePowerDelete(long entryId, Guid? userId)
        {
            ProfilePower? powerEntry = _context.ProfilePower
                .FirstOrDefault(pwr => pwr.Id == entryId && pwr.UserID == userId);

            if (powerEntry is null)
            {
                throw new AppException("Entry not found.");
            }
            
            _context.ProfilePower.Remove(powerEntry);
            _context.SaveChanges();

            return "Deleted";
        } 

        //TODO poprawic
        public AthleteData GetProfileData(Guid? userId)
        {
            StravaProfile? profile = GetUserById(userId).StravaProfile;
            if (profile is null) return null;

            List<ProfileHeartRate>? heartRate = _context.ProfileHeartRate
                .Where(phr => phr.UserID == userId)
                .ToList();

            List<ProfilePower>? power = _context.ProfilePower
                .Where(pp => pp.UserID == userId)
                .ToList();

            List<int>? years = _context.ProfileYearlySummary
                .OrderByDescending(y => y.Year)
                .Select(y => y.Year)
                .ToList();
            
            AthleteData response = new AthleteData
            {
                StravaProfileInfo = profile,
                HrZones = heartRate,
                PowerZones = power,
                YearsAvailable = years
            };
            
            return response;
        }

        //TODO zmienic na async
        public List<ProfileMonthlySummary> GetMonthlyStats(Guid? userId, int yearOffset)
        {
            List<ProfileMonthlySummary> monthlySummaries = _context.ProfileMonthlySummary
                .Where(summ => summ.UserId == userId && summ.Year == DateTime.Today.AddYears(-yearOffset).Year)
                .OrderBy(summ => summ.Month)
                .ToList();
            
            return monthlySummaries;
        }
        
        //TODO zmienic na async
        public IEnumerable<Activity> GetAthleteActivities(Guid? userId, DateTime? lastActivityDate, int? perPage)
        {
            perPage ??= 10;
            var activities = GetSyncedActivities((Guid)userId, (DateTime)lastActivityDate, (int)perPage);

            return activities;
        }

        public IEnumerable<Activity> GetAthletePeriodActivities(Guid? userId, int? month, int? yearOffset)
        {
            month ??= DateTime.UtcNow.Month;
            yearOffset ??= 0;
            
            DateTime first = new DateTime(DateTime.UtcNow.Year, (int)month, 1).AddYears(-(int)yearOffset).ToUniversalTime();
            DateTime last = first.AddMonths(1).AddSeconds(-1).ToUniversalTime();
            Console.WriteLine(first);
            
            var activities = _context.Activity
                .Where(a => a.UserId == userId && a.StartDate > first && a.StartDate < last) 
                .Include(a=> a.Laps)
                .OrderByDescending(a=> a.StartDate)
                .ToList();

            return activities;
        }
        
        public DateTime GetLatestActivity(Guid? userId)
        {
            var date = _context.Activity
                .Where(sa => sa.UserId == userId)
                .OrderByDescending(sa => sa.StartDate)
                .Select(sa => sa.StartDate)
                .FirstOrDefault();
            return date;
        }
        
        // helpers 
        //TODO poprawic to bo kilka razy jest ta sama funkcja w roznych plikach
        public User GetUserById(Guid? id)
        {
            User? user = _context.Users
                .Include(u => u.StravaProfile)
                .FirstOrDefault(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }

        public List<long> GetSyncedActivitiesId(Guid? id)
        {

            List<long>? userActivitiesId = _context.Activity
                .Where(sa => sa.UserId == id)
                .Select(sa => sa.StravaActivityID)
                .ToList();

            return userActivitiesId;
        }

        public List<Activity> GetSyncedActivities(Guid userId, DateTime date, int perPage)
        {
            var activities = _context.Activity
                .Where(a => a.UserId == userId && a.StartDate < date) 
                .Include(a=> a.Laps)
                .OrderByDescending(a=> a.StartDate)
                .Take(perPage)
                .ToList();

            return activities;
        }
    }
}
