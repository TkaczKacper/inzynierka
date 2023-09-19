using System.Text.Json.Serialization;

namespace server.Models.Strava
{
    public class StravaActivityLap
    {
        public long ID { get; set; }
        [JsonPropertyName("elapsed_time")]
        public int ElapsedTime { get; set; }
        [JsonPropertyName("moving_time")]
        public int MovingTime { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("distance")]
        public float Distance { get; set; }
        [JsonPropertyName("start_index")]
        public int StartIdx { get; set; }
        [JsonPropertyName("end_index")]
        public int EndIdx { get; set; }
        [JsonPropertyName("lap_index")]
        public int LapIndex { get; set; }
        [JsonPropertyName("total_elevation_gain")]
        public float TotalElevationGain { get; set; }
        [JsonPropertyName("average_speed")]
        public float AvgSpeed { get; set; }
        [JsonPropertyName("max_speed")]
        public float MaxSpeed { get; set; }
        [JsonPropertyName("average_watts")]
        public float AvgWatts { get; set; }
        [JsonPropertyName("average_cadence")]
        public float AvgCadence { get; set; }
        [JsonPropertyName("average_heartrate")]
        public float AvgHeartRate { get; set; }
        [JsonPropertyName("max_heartrate")]
        public float MaxHeartRate { get; set; }
    }
}
