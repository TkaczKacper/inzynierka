using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace server.Models.Authenticate
{
    public class AuthResponse
    {
        public int ID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateTime RegisterDate { get; set; }
        public required string JwtToken { get; set; }

        [JsonIgnore]
        public required string RefreshToken { get; set; }
    }
}
