using Microsoft.EntityFrameworkCore;
using server.Helpers;
using server.Models;
using server.Models.Strava;

namespace server.Services
{
    public interface IActivityService
    {
        Task<string> GetActivityDetails(string accessToken, Guid? userId);
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
            List<long> ids = user.StravaProfile.ActivitiesToFetch;
            List<long> activitiesAdded = new List<long>();

            List<StravaActivity> activities = new List<StravaActivity>();
            Console.WriteLine(stravaClient.DefaultRequestHeaders);
            if (!stravaClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                stravaClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accesstoken}");
            }
            Console.WriteLine(stravaClient.DefaultRequestHeaders);
            

            foreach (long id in ids)
            {
                var details = await _stravaApi.GetDetailsById(id, stravaClient);
                if (details.Manual) continue;
                var streams = await _stravaApi.GetStreamsById(id, stravaClient);
                if (details is null || streams is null) break;

                Console.WriteLine($"creating activity {id}");

                Dictionary<string, List<int>> intStreams = new();
                Dictionary<string, List<float>> floatStreams = new();
                List<bool> movingStream = new List<bool>();
                List<double> latStream = new List<double>();
                List<double> lngStream = new List<double>();
                List<StravaActivityLap> activityLaps = new List<StravaActivityLap>();
                foreach (var stream in streams)
                {
                    string type = stream.type;
                    if (type == "latlng")
                    {
                        foreach (object obj in stream.data)
                        {
                            string[] parts = obj.ToString().Trim('[', ']').Split(',');
                            latStream.Add(double.Parse(parts[0]));
                            lngStream.Add(double.Parse(parts[1]));
                        }
                        continue;
                    }
                    if (type == "moving")
                    {
                        foreach (object obj in stream.data)
                        {
                            movingStream.Add(bool.Parse(obj.ToString()));
                        }
                        continue;
                    }
                    if (type == "velocity_smooth" || type == "grade_smooth" || type == "distance" || type == "altitude")
                    {
                        floatStreams[type] = new List<float>();
                        foreach (object obj in stream.data)
                        {
                            floatStreams[type].Add(float.Parse(obj.ToString()));
                        }
                    }
                    else
                    {
                        intStreams[type] = new List<int>();
                        foreach (object obj in stream.data)
                        {
                            intStreams[type].Add(int.Parse(obj.ToString()));
                        }
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
                        TimeStream = intStreams.ContainsKey("time") ? intStreams["time"] : null,
                        Distance = floatStreams.ContainsKey("distance") ? floatStreams["distance"] : null,
                        Velocity = floatStreams.ContainsKey("velocity_smooth") ? floatStreams["velocity_smooth"] : null,
                        Watts = intStreams.ContainsKey("watts") ? intStreams["watts"] : null,
                        Cadence = intStreams.ContainsKey("cadence") ? intStreams["cadence"] : null,
                        HeartRate = intStreams.ContainsKey("heartrate") ? intStreams["heartrate"] : null,
                        Temperature = intStreams.ContainsKey("temp") ? intStreams["temp"] : null,
                        Altitude = floatStreams.ContainsKey("altitude") ? floatStreams["altitude"] : null,
                        GradeSmooth = floatStreams.ContainsKey("grade_smooth") ? floatStreams["grade_smooth"] : null,
                        Moving = movingStream,
                        Lat = latStream,
                        Lng = lngStream,
                        Laps = activityLaps,
                        UserProfile = user.StravaProfile
                        
                    };
                    int? HrRest = user.UserHeartRate?.LastOrDefault()?.HrRest;
                    int? HrMax = user.UserHeartRate?.LastOrDefault()?.HrMax;
                    if (details.Average_heartrate > 0 && HrMax is not null && HrRest is not null)
                    {
                        double multiplier = user.StravaProfile.Sex == "M" ? 1.92 : 1.67;
                        double trimp = 
                            details.Moving_time / 60 
                            * (details.Average_heartrate - (int)HrRest) / ((int)HrMax - (int)HrRest) 
                            * 64 
                            * Math.Exp(multiplier * (details.Average_heartrate - (int)HrRest) / ((int)HrMax - (int)HrRest));
                        activity.Trimp = trimp;
                    }
                    if (details.Device_watts && intStreams.ContainsKey("watts"))
                    {
                        int? userFtp = user.UserPower?.LastOrDefault()?.FTP;
                        int FTP = userFtp is null ? 250 : (int)userFtp;
                        List<double> avg = Enumerable.Range(0, intStreams["watts"]
                            .Count - 29).
                            Select(i => Math.Pow(intStreams["watts"].Skip(i).Take(30).Average(), 4)).ToList();

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
            user.StravaProfile.ActivitiesToFetch = remainingActivities;
            _context.SaveChanges();


            return $"{activities.Count} synced.";
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
    }
}
