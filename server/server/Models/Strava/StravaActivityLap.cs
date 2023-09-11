namespace server.Models.Strava
{
    public class StravaActivityLap
    {
        public int ID { get; set; }
        public int ElapsedTime { get; set; }
        public int MovingTime { get; set; }
        public int StartDate { get; set; }
        public int Distance { get; set; }
        public int StartIdx { get; set; }
        public int EndIdx { get; set; }
        public float TotalElevationGain { get; set; }
        public float AvgSpeed { get; set; }
        public float MaxSpeed { get; set; }
        public int AvgWatts { get; set; }
        public int AvgCadence { get; set; }
        public int AvgHeartRate { get; set; }
        public int LapIndex { get; set; }
    }
}
