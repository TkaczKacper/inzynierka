namespace server.Models.Responses.Strava.AthleteStats.cs;

public class StatsTotal
{
    public int count { get; set; }
    public float distance { get; set; }
    public int moving_time { get; set; }
    public int elapsed_time { get; set; }
    public float elevation_gain { get; set; }
    public int achievement_count { get; set; }
}