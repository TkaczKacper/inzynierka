using System.Text.Json.Serialization;

namespace server.Models;

public class StravaProfile
{
    public long ID { get; set; }
    [JsonIgnore]
    public string? StravaRefreshToken { get; set; }
   
    public long ProfileID { get; set; }
    
    public string? Username { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Sex { get; set; }
    
    public string? Bio { get; set; }
    
    public string? ProfileAvatar { get; set; }
    
    public string? Country { get; set; }
    
    public string? State { get; set; }
    
    public string? City { get; set; }
    
    public float Weight { get; set; }
    
    public DateTime ProfileCreatedAt { get; set; }
}