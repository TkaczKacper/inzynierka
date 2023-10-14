namespace server.Models.Responses.Strava.ActivityDetails
{
    public class ActivityLap
    {
        public int elapsed_time { get; set; }
        public int moving_time { get; set; }
        public DateTime start_date { get; set; }
        public float distance { get; set; }
        public int start_index { get; set; }
        public int end_index { get; set; }
        public int lap_index { get; set; }
        public float total_elevation_gain { get; set; }
        public float average_speed { get; set; }
        public float max_speed { get; set; }
        public float average_watts { get; set; }
        public float average_cadence { get; set; }
        public float average_heartrate { get; set; }
        public float max_heartrate { get; set; }
    }
}
