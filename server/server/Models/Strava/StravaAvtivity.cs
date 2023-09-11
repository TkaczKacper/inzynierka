namespace server.Models.Strava
{
    public class StravaAvtivity
    {
        public int ID { get; set; }
        public int StravaActivityID { get; set; }

        public string Title { get; set; }
        public int TotalDistance { get; set; }
        public int MovingTime { get; set; }
        public int ElapsedTime { get; set; }
        public float TotalElevationGain { get; set; }
        public int Calories { get; set; }
        public DateTime StratDate { get; set; }
        public List<double> StartLatLng { get; set; }
        public List<double> EndLatLng { get; set; }

        public float AvgSpeed { get; set; }
        public float MaxSpeed { get; set; }

        public int AvgHeartRate { get; set; }
        public int MaxHeartRate { get; set; }

        public bool Trainer { get; set; }
        public bool HasPowerMeter { get; set; }
        public int AvgWatts { get; set; }
        public int MaxWatts { get; set; }
        public int WeightedAvgWatts { get; set; }
        public int AvgCadence { get; set; }

        public int AvgTemp { get; set; }

        public float ElevationHigh { get; set; }
        public float ElevationLow { get; set; }

        public List<int> TimeStream { get; set; }
        public List<float> Distance { get; set; }
        public List<float> Velocity { get; set; }
        public List<int> Watts { get; set; }
        public List<int> Cadence { get; set; }
        public List<int> HeartRate { get; set; }
        public List<int> Temperature { get; set; }
        public List<float> Altitude { get; set; }
        public List<float> GradeSmooth { get; set; }
        public List<bool> Moving { get; set; }
        public List<LatLng> LatLng { get; set; }

        public List<StravaActivityLap> Laps { get; set; }
        public string Gear { get; set; }
        public string DeviceName { get; set; }
        
        public string SummaryPolyline { get; set; }
        public string DetailedPolyline { get; set; }
    }
}
