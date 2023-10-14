using System.Text.Json.Serialization;

namespace server.Models.Profile
{
    public class HrZones
    {
        [JsonIgnore]
        public long Id { get; set; }
        public int Zone1 { get; set; }
        public int Zone2 { get; set; }
        public int Zone3 { get; set; }
        public int Zone4 { get; set; }
        public int? Zone5 { get; set; }
        public int? Zone5a { get; set; }
        public int? Zone5b { get; set; }
        public int? Zone5c { get; set; }
    }
}
