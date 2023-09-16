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
        StravaProfile ProfileUpdate(StravaProfile profileInfo, Guid? id);
        Task<string> GetActivityDetails(string token);
    }

    public class StravaService : IStravaService
    {
        private DataContext _context;
        private readonly StravaSettings _stravaSettings;
        private static HttpClient stravaClient = new()
        {
            BaseAddress = new Uri("https://www.strava.com/api/v3/"),
        };

        public StravaService(DataContext context, IOptions<StravaSettings> stravaSettings)
        {
            _context = context;
            _stravaSettings = stravaSettings.Value;
        }

        public StravaProfile ProfileUpdate(StravaProfile profile, Guid? id)
        {
            Console.WriteLine("profile update");
            User? user = GetById(id);

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

        public async Task<string> GetActivityDetails(string accesstoken)
        {
            Console.WriteLine(_stravaSettings.ClientId);
            var xd = await GetActivityDetailsById(7521736676, accesstoken, stravaClient);
            var xd2 = await GetActivityStreamsById(7521736676, accesstoken, stravaClient);

            return xd2.ToString();
        }
        //helper methods
        public User GetById(Guid? id)
        {
            var user = _context.Users.Find(id);
            return user == null ? throw new KeyNotFoundException("User not found.") : user;
        }
        public async Task<object> GetActivityDetailsById(long id, string token, HttpClient httpClient)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                using HttpResponseMessage response = await httpClient.GetAsync($"activities/{id}");
                var jsonRes = await response.Content.ReadFromJsonAsync<ActivityDetailsResponse>();

                Console.WriteLine("test");
                Console.WriteLine(jsonRes?.Name);
                Console.WriteLine(jsonRes?.Max_heartrate);
                Console.WriteLine(jsonRes?.Start_date);
                Console.WriteLine(jsonRes?.Start_latlng?[1]);
                Console.WriteLine(response.RequestMessage);
                Console.WriteLine(response.StatusCode);
                return jsonRes;
            }
            catch (Exception ex){ Console.WriteLine(ex.Message); }
            return "";
        }
        public async Task<object> GetActivityStreamsById(long id, string token, HttpClient httpClient)
        {
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync($"activities/{id}/streams?keys=time,distance,latlng,altitude,velocity_smooth,heartrate,cadence,watts,temp,moving,grade_smooth&series_type=time");
                var jsonRes = await response.Content.ReadFromJsonAsync<List<Streams>>();
                Console.WriteLine(jsonRes[0].data);
                foreach(object i in jsonRes[0].data)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine(jsonRes[2].type);
                Console.WriteLine(jsonRes[4].type);
                Console.WriteLine(response.RequestMessage);
                Console.WriteLine(response.StatusCode);
                return jsonRes;
            }
             catch (Exception ex) { Console.WriteLine(ex.Message); }
            return "";
        }
    }
}
