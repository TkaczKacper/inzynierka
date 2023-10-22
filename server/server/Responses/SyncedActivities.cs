namespace server.Responses;

public class SyncedActivities
{
    public List<long> SyncedActivitiesId { get; set; }
    public DateTime LatestActivityDateTime { get; set; }
}