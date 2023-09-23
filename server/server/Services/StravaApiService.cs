using server.Models.Responses.Strava.ActivityStreams;
using server.Models.Responses.Strava;

namespace server.Services
{
    public interface IStravaApiService
    {
        Task<ActivityDetailsResponse> GetDetailsById(long id, HttpClient httpClient);
        Task<List<Streams>> GetStreamsById(long id, HttpClient httpClient);
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }
    }
}
