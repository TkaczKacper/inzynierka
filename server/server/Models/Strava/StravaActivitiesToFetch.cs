namespace server.Models.Strava
{
    public class StravaActivitiesToFetch
    {
        public long ID { get; set; }
        public List<long> activityIds { get; set; }
    }
}
