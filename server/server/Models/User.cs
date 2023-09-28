using Microsoft.EntityFrameworkCore;
using server.Models.Profile;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        public ProfileHeartRate? UserHeartRate { get; set; }
        public ProfilePower? UserPower { get; set; }
    }
    
}