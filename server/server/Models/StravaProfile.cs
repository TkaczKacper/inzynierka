using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using server.Models.Profile;

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
    
    [JsonIgnore]
    public StravaProfileStats? AthleteStats { get; set; }
    
    [JsonIgnore]
    [DefaultValue(true)]
    public bool NeedUpdate { get; set; }
    
    //foreign key
    [JsonIgnore]
    [ForeignKey("StravaProfileStats")]
    public virtual long AthleteStatsId { get; set; }
}