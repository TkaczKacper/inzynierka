using System.Text.Json.Serialization;

namespace server.Models.Profile;

public class StravaProfileStats
{
    [JsonIgnore]
    public long Id { get; set; }
    public double BiggestClimb { get; set; }
    public double LongestRide { get; set; }
    public RideTotals? RecentRideTotals { get; set; }
    public RideTotals? YtdRideTotals { get; set; }
    public RideTotals? AllTimeRideTotals { get; set; }
}
