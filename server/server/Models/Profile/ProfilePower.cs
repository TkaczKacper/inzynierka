﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace server.Models.Profile
{
    public class ProfilePower
    {
        public long Id { get; set; }
        public DateOnly DateAdded { get; set; }
        public int FTP { get; set; }
        public int Zone1 { get; set; }
        public int Zone2 { get; set; }
        public int Zone3 { get; set; }
        public int Zone4 { get; set; }
        public int Zone5 { get; set; }
        public int Zone6 { get; set; }
        public int Zone7 { get; set; }

        public bool SetAutoZones { get; set; } = true; 

        //foreign key property
        [JsonIgnore]
        [ForeignKey("Users")]
        public virtual Guid UserID { get; set; }
    }
}
