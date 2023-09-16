namespace server.Models.Strava
{
    public class StravaActivitiesToFetch
    {
        public long ID { get; set; }
        public List<long>? ActivityIds { get; set; }
    }
}
