using Microsoft.EntityFrameworkCore;
using server.Models.Profile;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using server.Models.Strava;

namespace server.Models
{
     public class User
     {
        public Guid ID { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public DateTime RegisterDate { get; set; }

        [JsonIgnore]
        public required string Password { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }

        public StravaProfile? StravaProfile { get; set; }

        public List<ProfileHeartRate>? UserHeartRate { get; set; }
        public List<ProfilePower>? UserPower { get; set; }
        
        [JsonIgnore]
        public virtual List<StravaActivity>? Activities { get; set; }
        
        [JsonIgnore]
        public List<long>? ActivitiesToFetch { get; set; }
    }
    
}