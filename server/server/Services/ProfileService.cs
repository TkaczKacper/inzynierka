using Microsoft.EntityFrameworkCore;
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
        Task<StravaProfile> ProfileUpdate(StravaProfile profileInfo, Guid userId, string refreshToken);
        Task<ProfileHeartRate> ProfileHeartRateUpdate(ProfileHeartRate profileHeartRate, Guid userId);
        Task<string> ProfileHeartRateDelete(long entryId, Guid userId);
        Task<ProfilePower> ProfilePowerUpdate(ProfilePower profilePower, Guid userId);
        Task<string> ProfilePowerDelete(long entryId, Guid userId);
        Task<AthleteData> GetProfileData(Guid userId);
        
        
        //TODO dodac serwis do obslugi danych uzytkownika
        Task<List<Activity>> GetAthletePeriodActivities(Guid userId, int? month, int? yearOffset);
    }

    public class ProfileService : IStravaService
    {
        private readonly DataContext _context;
        private readonly IHelperService _helperService;

        public ProfileService(DataContext context, IHelperService helperService)
        {
            _context = context;
            _helperService = helperService;
        }

        public async Task<StravaProfile> ProfileUpdate(StravaProfile profile, Guid id, string? refreshToken)
        {
            Console.WriteLine("profile update");
            User? user = await _helperService.GetDetailedUserById(id);
            
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

        public async Task<ProfileHeartRate> ProfileHeartRateUpdate(ProfileHeartRate profileHeartRate, Guid userId)
        {
            int? hrMax = profileHeartRate.HrMax;
            int? hrRest = profileHeartRate.HrRest;
            int? ltHr= profileHeartRate.LTHr;
            bool autoZones = profileHeartRate.SetAutoZones;

            ProfileHeartRate? existing = await _context.ProfileHeartRate
                .FirstOrDefaultAsync(x => x.DateAdded == DateOnly.FromDateTime(DateTime.UtcNow));
            
            ProfileHeartRate userHr = new ProfileHeartRate
            {
                UserID = userId
            };
            
            if (autoZones && hrMax != null && hrRest != null)
            {
                if (existing is not null)
                {
                    userHr.DateAdded = existing.DateAdded;
                    existing.HrRest = userHr.HrRest = profileHeartRate.HrRest;
                    existing.HrMax = userHr.HrMax = profileHeartRate.HrMax;
                    existing.LTHr = userHr.LTHr = profileHeartRate.LTHr;
                    existing.Zone1 = userHr.Zone1 = (int)hrRest;
                    if (ltHr != null)
                    {
                        existing.Zone2 = userHr.Zone2 = (int)(0.81 * ltHr);
                        existing.Zone3 = userHr.Zone3 = (int)(0.90 * ltHr);
                        existing.Zone4 = userHr.Zone4 = (int)(0.94 * ltHr);
                        existing.Zone5 = userHr.Zone5 = null;
                        existing.Zone5a = userHr.Zone5a = ltHr;
                        existing.Zone5b = userHr.Zone5b = (int)(1.03 * ltHr);
                        existing.Zone5c = userHr.Zone5c = (int)(1.06 * ltHr);
                    }
                    else
                    {
                        existing.LTHr = userHr.LTHr = null;
                        existing.Zone2 = userHr.Zone2 = (int)(0.65 * hrMax);
                        existing.Zone3 = userHr.Zone3 = (int)(0.77 * hrMax);
                        existing.Zone4 = userHr.Zone4 = (int)(0.86 * hrMax);
                        existing.Zone5 = userHr.Zone5 = (int)(0.935 * hrMax);
                    }

                    _context.ProfileHeartRate.Update(existing);
                }
                else
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
                    
                    await _context.ProfileHeartRate.AddAsync(userHr);
                }
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
                
                await _context.ProfileHeartRate
                    .Where(e => e.DateAdded == profileHeartRate.DateAdded)
                    .ExecuteUpdateAsync(hr => hr
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
            
            await _context.SaveChangesAsync();

            return userHr;
        }

        public async Task<string> ProfileHeartRateDelete(long entryId, Guid userId)
        {
            ProfileHeartRate? hrEntry = await _context.ProfileHeartRate
                .FirstOrDefaultAsync(hr => hr.ID == entryId && hr.UserID == userId);

            if (hrEntry is null)
            {
                throw new AppException("Entry not found.");
            }
            
            _context.ProfileHeartRate.Remove(hrEntry);
            await _context.SaveChangesAsync();

            return "Deleted";
        } 
        
        public async Task<ProfilePower> ProfilePowerUpdate(ProfilePower profilePower, Guid userId)
        {
            int ftp = profilePower.FTP;

            ProfilePower? existing = await _context.ProfilePower
                    .FirstOrDefaultAsync(x => x.DateAdded == DateOnly.FromDateTime(DateTime.UtcNow));
            
            ProfilePower power = new ProfilePower
            {
                FTP = ftp,
                UserID = userId,
            };

            if (profilePower.SetAutoZones)
            {
                if (existing is not null)
                {
                    power.DateAdded = existing.DateAdded;
                    existing.FTP = power.FTP = profilePower.FTP;
                    existing.Zone1 = power.Zone1 = 0;
                    existing.Zone2 = power.Zone2 = (int)Math.Floor(0.55 * ftp);
                    existing.Zone3 = power.Zone3 = (int)Math.Floor(0.75 * ftp);
                    existing.Zone4 = power.Zone4 = (int)Math.Floor(0.90 * ftp);
                    existing.Zone5 = power.Zone5 = (int)Math.Floor(1.05 * ftp);
                    existing.Zone6 = power.Zone6 = (int)Math.Floor(1.20 * ftp);
                    existing.Zone7 = power.Zone7 = (int)Math.Floor(1.75 * ftp);

                    _context.ProfilePower.Update(existing);
                }
                else
                {
                    power.DateAdded = DateOnly.FromDateTime(DateTime.UtcNow);
                    power.Zone1 = 0;
                    power.Zone2 = (int)Math.Floor(0.55 * ftp);
                    power.Zone3 = (int)Math.Floor(0.75 * ftp);
                    power.Zone4 = (int)Math.Floor(0.90 * ftp);
                    power.Zone5 = (int)Math.Floor(1.05 * ftp);
                    power.Zone6 = (int)Math.Floor(1.20 * ftp);
                    power.Zone7 = (int)Math.Floor(1.75 * ftp);
                
                    await _context.ProfilePower.AddAsync(power);
                }
            }
            else
            {
                power.DateAdded = profilePower.DateAdded;
                power.Zone1 = profilePower.Zone1;
                power.Zone2 = profilePower.Zone2;
                power.Zone3 = profilePower.Zone3;
                power.Zone4 = profilePower.Zone4;
                power.Zone5 = profilePower.Zone5;
                power.Zone6 = profilePower.Zone6;
                power.Zone7 = profilePower.Zone7;

                await _context.ProfilePower
                    .Where(e => e.DateAdded == profilePower.DateAdded)
                    .ExecuteUpdateAsync(up => up
                        .SetProperty(x => x.Zone1, profilePower.Zone1)
                        .SetProperty(x => x.Zone2, profilePower.Zone2)
                        .SetProperty(x => x.Zone3, profilePower.Zone3)
                        .SetProperty(x => x.Zone4, profilePower.Zone4)
                        .SetProperty(x => x.Zone5, profilePower.Zone5)
                        .SetProperty(x => x.Zone6, profilePower.Zone6)
                        .SetProperty(x => x.Zone7, profilePower.Zone7)
                    );
            }

            await _context.SaveChangesAsync();

            return power;
        }
        
        public async Task<string> ProfilePowerDelete(long entryId, Guid userId)
        {
            ProfilePower? powerEntry = await _context.ProfilePower
                .FirstOrDefaultAsync(pwr => pwr.Id == entryId && pwr.UserID == userId);

            if (powerEntry is null)
            {
                throw new AppException("Entry not found.");
            }
            
            _context.ProfilePower.Remove(powerEntry);
            await _context.SaveChangesAsync();

            return "Deleted";
        } 

        public async Task<AthleteData> GetProfileData(Guid userId)
        {
            User? user = await _helperService.GetDetailedUserById(userId);
            StravaProfile? profile = user?.StravaProfile;
            if (profile is null) return null;

            List<ProfileHeartRate>? heartRate = await _helperService.GetAthleteHeartRate(userId);

            List<ProfilePower>? power = await _helperService.GetAthletePower(userId);

            List<ProfileYearlySummary> yearlySummaries = await  _helperService.GetAthleteYearlySummary(userId);
            List<int>? years = yearlySummaries.Select(x => x.Year).ToList();
            
            AthleteData response = new AthleteData
            {
                StravaProfileInfo = profile,
                HrZones = heartRate,
                PowerZones = power,
                YearsAvailable = years
            };
            
            return response;
        }

        public async Task<List<Activity>> GetAthletePeriodActivities(Guid userId, int? month, int? yearOffset)
        {
            month ??= DateTime.UtcNow.Month;
            yearOffset ??= 0;
            
            DateTime first = new DateTime(DateTime.UtcNow.Year, (int)month, 1).AddYears(-(int)yearOffset).ToUniversalTime();
            DateTime last = first.AddMonths(1).AddSeconds(-1).ToUniversalTime();
            Console.WriteLine(first);

            var activities = await _helperService.GetAthleteActivitiesInDateRange(userId, first, last);
            
            return activities;
        }
    }
}
