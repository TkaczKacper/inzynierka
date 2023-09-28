﻿namespace server.Models.Profile
{
    public class ProfileHeartRate
    {
        public long ID { get; set; }
        public DateOnly DateAdded { get; set; }
        public int? HrRest { get; set; }
        public int? HrMax { get; set; }
        public int? LTHr { get; set; }
        public HrZones? HrZones { get; set; }
    }
}
