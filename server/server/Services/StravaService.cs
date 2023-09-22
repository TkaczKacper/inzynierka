using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Helpers;
using server.Models;
using server.Models.Responses.Strava;
using server.Models.Responses.Strava.ActivityStreams;
using server.Models.Strava;
using System.IO;

namespace server.Services
{
    public interface IStravaService
    {
        StravaProfile ProfileUpdate(StravaProfile profileInfo, Guid? userId);
        Task<StravaActivity> GetActivityDetails(string token, Guid? userId);
        string SaveActivitiesToFetch(List<long> activityIds, Guid? userId);
    }

    public class StravaService : IStravaService
    {
        private readonly StravaSettings _stravaSettings;
        private DataContext _context;
        private static HttpClient stravaClient = new()
        {
            BaseAddress = new Uri("https://www.strava.com/api/v3/"),
        };

        public StravaService(
            IOptions<StravaSettings> stravaSettings,
            DataContext context)
        {
            _stravaSettings = stravaSettings.Value;
            _context = context;
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

        public string SaveActivitiesToFetch(List<long> activities, Guid? userId)
        {
            Console.WriteLine("Saving...");
            
            User user = GetUserById(userId);
            user.StravaProfile.ActivitiesToFetch = activities;
            _context.Update(user);
            _context.SaveChanges();
            Console.WriteLine($"profile: {user.StravaProfile.LastName}");

            return $"Remaining activities to be fetch: {activities.Count}";
        }

        public async Task<StravaActivity> GetActivityDetails(string accesstoken, Guid? userId)
        {
            User user = await GetUserByIdAsync(userId);
            StravaProfile stravaProfile = await GetStravaProfileAsync(user.StravaProfile.ID);

            var details = await GetActivityDetailsById(7521736676, accesstoken, stravaClient);
            var streams = await GetActivityStreamsById(7521736676, accesstoken, stravaClient);


            Console.WriteLine("creating activity");

            Dictionary<string, List<int>> intStreams = new();
            Dictionary<string, List<float>> floatStreams = new();
            List<bool> movingStream = new List<bool>();
            List<double> latStream = new List<double>();
            List<double> lngStream = new List<double>();
            List<StravaActivityLap> activityLaps = new List<StravaActivityLap>();
            foreach (var stream in streams)
            {
                string type = stream.type;
                Console.WriteLine("foreach" + type);
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
                    Console.WriteLine("XD");
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

            Console.WriteLine($"test2: {details.Laps[0].average_speed}, {details.Average_speed}");
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
                    Laps = activityLaps
                };

                user.StravaProfile.Activities.Add(activity);
                _context.SaveChanges();


                return activity;
            }
             catch (Exception ex) { Console.WriteLine("ERROR" + ex.Message); }

            
            return null;
        }

        public async Task<ActivityDetailsResponse> GetActivityDetailsById(long id, string token, HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                using HttpResponseMessage response = await httpClient.GetAsync($"activities/{id}");
                var jsonRes = await response.Content.ReadFromJsonAsync<ActivityDetailsResponse>();

                Console.WriteLine("test");
                Console.WriteLine(jsonRes?.Average_speed);
                Console.WriteLine(jsonRes?.Name);
                Console.WriteLine(jsonRes?.Max_heartrate);
                Console.WriteLine(jsonRes?.Start_date);
                Console.WriteLine(jsonRes?.Start_latlng?[1]);
                Console.WriteLine($"Lap: {jsonRes?.Laps[0].average_speed}");

                return jsonRes;
            }
            catch (Exception ex){ Console.WriteLine(ex.Message); }
            return null;
        }
        public async Task<List<Streams>> GetActivityStreamsById(long id, string token, HttpClient httpClient)
        {
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync($"activities/{id}/streams?keys=time,distance,latlng,altitude,velocity_smooth,heartrate,cadence,watts,temp,moving,grade_smooth&series_type=time");
                var jsonRes = await response.Content.ReadFromJsonAsync<List<Streams>>();
                Console.WriteLine(jsonRes[0].data);
                Console.WriteLine(jsonRes[2].type);
                Console.WriteLine(jsonRes[4].type);

                return jsonRes;
            }
             catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }

        public string AddLaps(long activityId, List<StravaActivityLap> laps)
        {
            StravaActivity activityUpdate = _context.StravaActivity.FirstOrDefault(a => a.StravaActivityID == activityId);

            activityUpdate.Laps = laps;
            _context.Update(activityUpdate);
            _context.SaveChanges();

            return "OK";
        }


        // helpers 
        public User GetUserById(Guid? id)
        {
            User? user = _context.Users.Include(u => u.StravaProfile).Include(u => u.StravaProfile.Activities).FirstOrDefault(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
        public async Task<User> GetUserByIdAsync(Guid? id)
        {
            User? user = await _context.Users.
                Include(u => u.StravaProfile).
                ThenInclude(s => s.Activities).
                ThenInclude(a => a.Laps).
                FirstOrDefaultAsync(u => u.ID == id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
        public async Task<StravaProfile> GetStravaProfileAsync(long id)
        {
            StravaProfile profile = _context.StravaProfile.Include(u => u.Activities).FirstOrDefault(u => u.ID == id);
            profile.Version = Guid.NewGuid();
            _context.SaveChanges();
            return profile == null ? throw new KeyNotFoundException("User not found.") : profile;
        }

        public string SaveDb(StravaActivity activity, User user)
        {
            try
            {
                user.StravaProfile.Activities.Add(activity);
                _context.SaveChanges();
                return $"Activity {activity.StravaActivityID} saved";
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine("Error" + ex.Message); 
                return "Something went wrong"; 
            }
        }
    }
}
