﻿using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Profile
{
    public class ProfilePower
    {
        public long Id { get; set; }
        public DateOnly DateAdded { get; set; }
        public int? FTP { get; set; }
        public int? Zone1 { get; set; }
        public int? Zone2 { get; set; }
        public int? Zone3 { get; set; }
        public int? Zone4 { get; set; }
        public int? Zone5 { get; set; }
        public int? Zone6 { get; set; }
        public int? Zone7 { get; set; }

        //foreign key property
        [ForeignKey("Users")]
        public virtual Guid UserId { get; set; }
    }
}
