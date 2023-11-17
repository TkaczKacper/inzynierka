using System.ComponentModel.DataAnnotations.Schema;
using server.Models.Profile;

namespace server.Models.Strava
{
    public class StravaActivity
    {
        public long ID { get; set; }
        public long StravaActivityID { get; set; }
        public string? Title { get; set; }
        public string? Type { get; set; }
        public string? SportType { get; set; }
        public float TotalDistance { get; set; }
        public int MovingTime { get; set; }
        public int ElapsedTime { get; set; }
        public float TotalElevationGain { get; set; }
        public float Calories { get; set; }
        public DateTime StartDate { get; set; }
        public double[]? StartLatLng { get; set; }
        public double[]? EndLatLng { get; set; }

        public float AvgSpeed { get; set; }
        public float MaxSpeed { get; set; }

        public float AvgHeartRate { get; set; }
        public int MaxHeartRate { get; set; }
        public bool Trainer { get; set; }
        public bool HasPowerMeter { get; set; }
        public float AvgWatts { get; set; }
        public int MaxWatts { get; set; }
        public int WeightedAvgWatts { get; set; }
        public float AvgCadence { get; set; }
        public int MaxCadence {get; set; }
        public float AvgTemp { get; set; }
        public int MaxTemp { get; set; }
        public float ElevationHigh { get; set; }
        public float ElevationLow { get; set; }
        public List<StravaActivityLap>? Laps { get; set; }
        public string? Gear { get; set; }
        public string? DeviceName { get; set; }
        public string? SummaryPolyline { get; set; }
        public string? DetailedPolyline { get; set; }
        public int Achievements { get; set; }
        
        
        public double? Trimp { get; set; }
        public double? NormalizedPower { get; set; }
        public double? IntensityFactor { get; set; }
        public double? VariabilityIndex { get; set; }
        public double? Tss { get; set; }
        
        public StravaActivityStreams ActivityStreams { get; set; }
        public List<int>? PowerCurve { get; set; }
        
        public TimeInHrZone? HrTimeInZone { get; set; }
        public TimeInPowerZone? PowerTimeInZone { get; set; }

        //foreign key property
        [ForeignKey("User")]
        public virtual Guid UserId { get; set; }
        public virtual required User UserProfile { get; set; }
    }
}
