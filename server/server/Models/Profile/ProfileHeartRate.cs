using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace server.Models.Profile
{
    public class ProfileHeartRate
    {
        [JsonIgnore]
        public long ID { get; set; }
        public DateOnly DateAdded { get; set; }
        public int? HrRest { get; set; }
        public int? HrMax { get; set; }
        public int? LTHr { get; set; }
        
        public int Zone1 { get; set; }
        public int Zone2 { get; set; }
        public int Zone3 { get; set; }
        public int Zone4 { get; set; }
        public int? Zone5 { get; set; }
        public int? Zone5a { get; set; }
        public int? Zone5b { get; set; }
        public int? Zone5c { get; set; }

        //foreign key property
        [JsonIgnore]
        [ForeignKey("Users")]
        public virtual Guid UserID { get; set; }
    }
}
