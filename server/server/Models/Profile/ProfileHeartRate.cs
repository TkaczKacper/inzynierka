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
        public HrZones? HrZones { get; set; }

        //foreign key property
        [ForeignKey("Users")]
        public virtual Guid UserID { get; set; }
    }
}
