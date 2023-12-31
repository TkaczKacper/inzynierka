﻿using server.Responses.Strava.ActivityDetails;

namespace server.Responses.Strava
{
    public class ActivityDetailsResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public float Distance { get; set; }
        public string? Type { get; set; }
        public string? Sport_type { get; set; }
        public int Moving_time { get; set; }
        public int Elapsed_time { get; set; }
        public float Total_elevation_gain { get; set; }
        public float Calories { get; set; }
        public DateTime Start_date { get; set; }
        public double[]? Start_latlng { get; set; }
        public double[]? End_latlng { get; set; }
        public float Average_speed { get; set; }
        public float Max_speed { get; set; }
        public float Average_heartrate { get; set; }
        public float Max_heartrate { get; set; }
        public bool Trainer { get; set; }
        public bool Device_watts { get; set; }
        public float Average_watts { get; set; }
        public int Max_watts { get; set; }
        public int Weighted_average_watts { get; set; }
        public float Kilojules { get; set; }
        public float Average_cadence { get; set; }
        public float Average_temp { get; set; }
        public float Elev_high { get; set; }
        public float Elev_low { get; set; }
        public string? Device_name { get; set; }

        public ActivityLap[]? Laps { get; set; }
        public ActivityMap? Map { get; set; }
        public ActivityGear? Gear { get; set; }
        public int Achievement_count { get; set; }
        public bool Manual { get; set; }
    }
}
