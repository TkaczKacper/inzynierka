using server.Models.Profile;
using server.Models.Responses.Strava.ActivityStreams;
using server.Models.Responses.Strava;
using server.Models.Responses.Strava.AthleteStats.cs;

namespace server.Services
{
    public interface IStravaApiService
    {
        Task<ActivityDetailsResponse> GetDetailsById(long id, HttpClient httpClient);
        Task<List<Streams>> GetStreamsById(long id, HttpClient httpClient);
        Task<StravaProfileStats> GetAthleteStats(long id, HttpClient httpClient);
    }

    public class StravaApiService : IStravaApiService
    {


        public async Task<ActivityDetailsResponse> GetDetailsById(long id, HttpClient stravaClient)
        {
            try
            {
                using HttpResponseMessage response = await stravaClient.GetAsync($"activities/{id}");
                var detailsResponse = await response.Content.ReadFromJsonAsync<ActivityDetailsResponse>();

                return detailsResponse;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"error: {ex.Message}");
                Console.WriteLine("Strava API limit reached. Try again later."); 
            }
            return null;
        }

        public async Task<List<Streams>> GetStreamsById(long id, HttpClient stravaClient)
        {
            try
            {
                using HttpResponseMessage response = await stravaClient.GetAsync($"activities/{id}/streams?keys=time,distance,latlng,altitude,velocity_smooth,heartrate,cadence,watts,temp,moving,grade_smooth&series_type=time");
                var streamsResponse = await response.Content.ReadFromJsonAsync<List<Streams>>();

                return streamsResponse;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"error: {ex.Message}");
                Console.WriteLine("Strava API limit reached. Try again later."); 
            }
            return null;
        }

        public async Task<StravaProfileStats> GetAthleteStats(long id, HttpClient stravaClient)
        {
            try
            {
                using HttpResponseMessage response = await stravaClient.GetAsync($"athletes/{id}/stats");
                var stats = await response.Content.ReadFromJsonAsync<AthleteStats>();

                RideTotals recent = new RideTotals
                {
                    Count = stats.recent_ride_totals.count,
                    Distance = stats.recent_ride_totals.distance,
                    ElevationGain = stats.recent_ride_totals.elevation_gain,
                    ElapsedTime = stats.recent_ride_totals.elapsed_time,
                    MovingTime = stats.recent_ride_totals.moving_time,
                    AchievementCount = stats.recent_ride_totals.achievement_count
                };
                
                RideTotals ytd = new RideTotals
                {
                    Count = stats.ytd_ride_totals.count,
                    Distance = stats.ytd_ride_totals.distance,
                    ElevationGain = stats.ytd_ride_totals.elevation_gain,
                    ElapsedTime = stats.ytd_ride_totals.elapsed_time,
                    MovingTime = stats.ytd_ride_totals.moving_time,
                    AchievementCount = stats.ytd_ride_totals.achievement_count
                };
                
                RideTotals total = new RideTotals
                {
                    Count = stats.all_ride_totals.count,
                    Distance = stats.all_ride_totals.distance,
                    ElevationGain = stats.all_ride_totals.elevation_gain,
                    ElapsedTime = stats.all_ride_totals.elapsed_time,
                    MovingTime = stats.all_ride_totals.moving_time,
                    AchievementCount = stats.all_ride_totals.achievement_count
                };
                StravaProfileStats athleteStats = new StravaProfileStats
                {
                    BiggestClimb = stats.biggest_climb_elevation_gain,
                    LongestRide = stats.biggest_ride_distance,
                    RecentRideTotals = recent,
                    YtdRideTotals = ytd,
                    AllTimeRideTotals = total
                };
                return athleteStats;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
            }

            return null;
        }
    }
}
