using server.Models.Responses.Strava.ActivityDetails;
using server.Models.Strava;

namespace server.Models.Responses.Strava
{
    public class ActivityDetailsResponse
    {
        public long id { get; set; }
        public string Name { get; set; }
        public float distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public float total_eleavtion_gain { get; set; }
        public float calories { get; set; }
        public DateTime start_date { get; set; }
        public double[] start_latlng { get; set; }
        public double[] end_latlng { get; set; }
        public float average_speed { get; set; }
        public float max_speed { get; set; }
        public float average_heartrate { get; set; }
        public float max_heartrate { get; set; }
        public bool trainer { get; set; }
        public bool device_watts { get; set; }
        public float average_watts { get; set; }
        public int max_watts { get; set; }
        public int weighted_average_watts { get; set; }
        public float kilojules { get; set; }
        public float average_cadence { get; set; }
        public float average_temp { get; set; }
        public float elev_high { get; set; }
        public float elev_low { get; set; }


        public ActivityMap map { get; set; }
        public ActivityGear gear { get; set; }
        public int achievement_count { get; set; }
    }
}
