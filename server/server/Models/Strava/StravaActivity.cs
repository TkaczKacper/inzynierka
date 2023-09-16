using System.Text.Json.Serialization;

namespace server.Models.Strava
{
    public class StravaActivity
    {
        public long ID { get; set; }
        public long StravaActivityID { get; set; }
        public string? Title { get; set; }
        public float TotalDistance { get; set; }
        public int MovingTime { get; set; }
        public int ElapsedTime { get; set; }
        public float TotalElevationGain { get; set; }
        public float Calories { get; set; }
        public DateTime StratDate { get; set; }
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
        public float AvgTemp { get; set; }
        public float ElevationHigh { get; set; }
        public float ElevationLow { get; set; }
        public List<StravaActivityLap>? Laps { get; set; }
        public string? Gear { get; set; }
        public string? DeviceName { get; set; }
        public string? SummaryPolyline { get; set; }
        public string? DetailedPolyline { get; set; }
        public int Achievements { get; set; }

        public int[]? TimeStream { get; set; }
        public float[]? Distance { get; set; }
        public float[]? Velocity { get; set; }
        public int[]? Watts { get; set; }
        public int[]? Cadence { get; set; }
        public int[]? HeartRate { get; set; }
        public int[]? Temperature { get; set; }
        public float[]? Altitude { get; set; }
        public float[]? GradeSmooth { get; set; }
        public bool[]? Moving { get; set; }
        public double[]? LatLng { get; set; }
    }
}
