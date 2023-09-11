namespace server.Models.Strava
{
    public class StravaActivitiesToFetch
    {
        public int ID { get; set; }
        public List<int> activityIds { get; set; }
    }
}
