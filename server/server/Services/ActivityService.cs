using Microsoft.EntityFrameworkCore;
using server.Helpers;
using server.Models;
using server.Models.Strava;
using System.Globalization;
using server.Models.Profile;

namespace server.Services
{
    public interface IActivityService
    {
        Task<string> GetActivityDetails(string accessToken, Guid? userId);
        StravaActivity GetActivityById(long activityId);
    }

    public class ActivityService : IActivityService
    {
        private readonly DataContext _context;
        private IStravaApiService _stravaApi;

        public ActivityService(
            DataContext context,
            IStravaApiService stravaApi)
        {
            _context = context;
            _stravaApi = stravaApi;
        }

        private static HttpClient stravaClient = new()
        {
            BaseAddress = new Uri("https://www.strava.com/api/v3/"),
        };

        public async Task<string> GetActivityDetails(string accesstoken, Guid? userId)
        {
            User user = await GetUserByIdAsync(userId);
            List<long> ids = user.ActivitiesToFetch;
            List<long> activitiesAdded = new List<long>();

            List<StravaActivity> activities = new List<StravaActivity>();
            Console.WriteLine(stravaClient.DefaultRequestHeaders);
            if (!stravaClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                stravaClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accesstoken}");
            }
            Console.WriteLine(stravaClient.DefaultRequestHeaders);
            int? HrRest = user.UserHeartRate?.LastOrDefault()?.HrRest;
            int? HrMax = user.UserHeartRate?.LastOrDefault()?.HrMax;
            int? userFtp = user.UserPower?.LastOrDefault()?.FTP;

            foreach (long id in ids)
            {
                var details = await _stravaApi.GetDetailsById(id, stravaClient);
                if (details.Manual)
                {
                    activitiesAdded.Add(id);
                    continue;
                };
                
                StravaActivityStreams streams = await _stravaApi.GetStreamsById(id, stravaClient);
                
                if (details is null || streams is null) break;

                Console.WriteLine($"creating activity {id}");

                List<StravaActivityLap> activityLaps = new List<StravaActivityLap>();

                List<int> activityPowerCurve = new List<int>();
                if (streams.Watts.Count > 0)
                {
                    for (int i = 1; i < streams.Watts.Count; i++)
                    {
                        activityPowerCurve.Add(CalculateActivityPowerCurve(streams.Watts, i));
                    }
                }

                foreach (var lap in details.Laps)
                {
                    StravaActivityLap activityLap = new StravaActivityLap()
                    {
                        ElapsedTime = lap.elapsed_time,
                        MovingTime = lap.moving_time,
                        StartDate = lap.start_date,
                        Distance = lap.distance,
                        StartIdx = lap.start_index,
                        EndIdx = lap.end_index,
                        LapIndex = lap.lap_index,
                        TotalElevationGain = lap.total_elevation_gain,
                        AvgSpeed = lap.average_speed,
                        MaxSpeed = lap.max_speed,
                        AvgWatts = lap.average_watts,
                        AvgCadence = lap.average_cadence,
                        AvgHeartRate = lap.average_heartrate,
                        MaxHeartRate = lap.max_heartrate
                    };
                    activityLaps.Add(activityLap);
                }

                try
                {
                    StravaActivity activity = new StravaActivity
                    {
                        StravaActivityID = details.Id,
                        Title = details.Name,
                        TotalDistance = details.Distance,
                        MovingTime = details.Moving_time,
                        ElapsedTime = details.Elapsed_time,
                        TotalElevationGain = details.Total_elevation_gain,
                        Calories = details.Calories,
                        StartDate = details.Start_date,
                        StartLatLng = details.Start_latlng,
                        EndLatLng = details.End_latlng,
                        AvgSpeed = details.Average_speed,
                        MaxSpeed = details.Max_speed,
                        AvgHeartRate = details.Average_heartrate,
                        MaxHeartRate = (int)details.Max_heartrate,
                        Trainer = details.Trainer,
                        HasPowerMeter = details.Device_watts,
                        AvgWatts = details.Average_watts,
                        MaxWatts = details.Max_watts,
                        WeightedAvgWatts = details.Weighted_average_watts,
                        AvgCadence = details.Average_cadence,
                        AvgTemp = details.Average_temp,
                        ElevationHigh = details.Elev_high,
                        ElevationLow = details.Elev_low,
                        Gear = details.Gear?.name,
                        DeviceName = details.Device_name,
                        SummaryPolyline = details.Map?.summary_polyline,
                        DetailedPolyline = details.Map?.polyline,
                        Achievements = details.Achievement_count,
                        ActivityStreams = streams,
                        Laps = activityLaps,
                        PowerCurve = activityPowerCurve,
                        UserProfile = user
                    };
                    if (details.Average_heartrate > 0 && HrMax is not null && HrRest is not null && streams.HeartRate?.Count > 0)
                    {
                        TimeInHrZone timeInHrZones = CalculateTimeInHrZones(streams.HeartRate, streams.Moving, userId);
                        double multiplier = user.StravaProfile.Sex == "M" ? 1.92 : 1.67;
                        double trimp = 
                            details.Moving_time / 60 
                            * (details.Average_heartrate - (int)HrRest) / ((int)HrMax - (int)HrRest) 
                            * 0.64 
                            * Math.Exp(multiplier * (details.Average_heartrate - (int)HrRest) / ((int)HrMax - (int)HrRest));
                        activity.Trimp = trimp;
                        activity.HrTimeInZone = timeInHrZones;
                    }
                    if (details.Device_watts && streams.Watts?.Count > 0) 
                    {
                        int FTP = userFtp is null ? 250 : (int)userFtp;
                        List<double> avg = Enumerable.Range(0, streams.Watts 
                            .Count - 29).
                            Select(i => Math.Pow(streams.Watts.Skip(i).Take(30).Average(), 4)).ToList();

                        double NormalizedPower = Math.Pow(avg.Average(), 0.25);
                        double IntensityFactor = NormalizedPower / FTP;
                        double VariabilityIndex = NormalizedPower / details.Average_watts;
                        double Tss = (details.Moving_time * NormalizedPower * IntensityFactor) / (329 * 36);

                        activity.NormalizedPower = NormalizedPower;
                        activity.IntensityFactor = IntensityFactor;
                        activity.VariabilityIndex = VariabilityIndex;
                        activity.Tss = Tss;
                    }
                    activities.Add(activity);
                    activitiesAdded.Add(id);
                }
                catch (Exception ex) { Console.WriteLine("ERROR" + ex.Message); }
            }
            
            _context.StravaActivity.AddRange(activities);
            _context.SaveChanges();

            List<long> remainingActivities = ids.Where(id => !activitiesAdded.Contains(id)).ToList();
            user.ActivitiesToFetch = remainingActivities;
            _context.SaveChanges();


            return $"{activities.Count} synced.";
        }

        public StravaActivity GetActivityById(long activityId)
        {
            var activity = _context.StravaActivity.FirstOrDefault(sa => sa.ID == activityId);

            return activity == null ? throw new KeyNotFoundException("Activity not found.") : activity;
        }

        private async Task<User> GetUserByIdAsync(Guid? id)
        {
            User? user = await _context.Users.
                Include(u => u.StravaProfile).
                Include(u => u.UserHeartRate).
                Include(u => u.UserPower).
                FirstOrDefaultAsync(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }

        private int CalculateActivityPowerCurve(List<int> watts, int k)
        {
            int max = 0, curr_sum = 0;
            //int curr_start = 0, start = 0, end = 0;

            for (int i = 0; i < watts.Count; i++)
            {
                curr_sum += watts[i];

                if (i >= k)
                    curr_sum -= watts[i - k];

                if (i >= k - 1)
                {
                    if (curr_sum > max)
                    {
                        max = curr_sum;
                        // end = curr_start;
                        // start = curr_start - k + 1;
                    }
                }

                //curr_start++;
            }

            return max / k;
        }

        private TimeInHrZone? CalculateTimeInHrZones(List<int> hr, List<bool> moving, Guid? userId)
        {
            ProfileHeartRate? hrZones = _context.ProfileHeartRate
                .FirstOrDefault(hr => hr.UserID == userId);
            
            int Zone1 = 0;
            int Zone2 = 0;
            int Zone3 = 0;
            int Zone4 = 0;
            int Zone5 = 0;
            
            for (int i = 0; i < hr.Count; i++)
            {
                if (moving[i])
                {
                    if (hr[i] >= hrZones.Zone5) Zone5++;
                    if (hr[i] >= hrZones.Zone4 && hr[i] < hrZones.Zone5) Zone4++;
                    if (hr[i] >= hrZones.Zone3 && hr[i] < hrZones.Zone4) Zone3++;
                    if (hr[i] >= hrZones.Zone2 && hr[i] < hrZones.Zone3) Zone2++;
                    if (hr[i] >= hrZones.Zone1 && hr[i] < hrZones.Zone2) Zone1++;
                }
            }

            TimeInHrZone timeInHrZone = new TimeInHrZone
            {
                TimeInZ1 = Zone1,
                TimeInZ2 = Zone2,
                TimeInZ3 = Zone3,
                TimeInZ4 = Zone4,
                TimeInZ5 = Zone5
            };

            return timeInHrZone;
        }
    }
}
