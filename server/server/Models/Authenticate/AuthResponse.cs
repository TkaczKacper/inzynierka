using System.Text.Json.Serialization;

namespace server.Models.Authenticate
{
    public class AuthResponse
    {
        public Guid ID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateTime RegisterDate { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
        public string? StravaRefreshToken { get; set; }
        public AuthResponse(User user, string jwtToken, string refreshToken, string? stravaRefreshToken) 
        {
            ID = user.ID;
            Username = user.Username;
            Email = user.Email;
            RegisterDate = user.RegisterDate;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
            StravaRefreshToken = stravaRefreshToken;
        }
    }
}
