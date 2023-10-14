namespace server.Models.Responses.Strava.AthleteStats.cs;

public class AthleteStats
{
    public double biggest_ride_distance { get; set; }
    public double biggest_climb_elevation_gain { get; set; }
    public StatsTotal recent_ride_totals { get; set; }
    public StatsTotal ytd_ride_totals { get; set; }
    public StatsTotal all_ride_totals { get; set; }
}
