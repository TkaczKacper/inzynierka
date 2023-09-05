namespace server.Models;

public class StravaProfile
{
    public int ID { get; set; }
    public string StravaRefreshToken { get; set; }
    public int ProfileID { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Sex { get; set; }
    public string Bio { get; set; }
    public string ProfileAvatar { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public float Weight { get; set; }
    public DateTime ProfileCreatedAt { get; set; }
}