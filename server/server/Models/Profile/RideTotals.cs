namespace server.Models.Profile;

public class RideTotals
{
   public long Id { get; set; }
   public int Count { get; set; }
   public float Distance { get; set; }
   public int MovingTime { get; set; }
   public int ElapsedTime { get; set; }
   public float ElevationGain { get; set; }
   public int AchievementCount { get; set; }
}