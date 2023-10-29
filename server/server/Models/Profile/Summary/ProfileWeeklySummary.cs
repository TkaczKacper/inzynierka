using System.ComponentModel;

namespace server.Models.Profile.Summary;

public class ProfileWeeklySummary
{
    public long Id { get; set; }
    public int Year { get; set; }
    public int Week { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public float TotalDistance { get; set; }
    public float TotalElevationGain { get; set; }
    public float TotalCalories { get; set; }
    public int TotalMovingTime { get; set; }
    public int TotalElapsedTime { get; set; }
    public double TrainingLoad { get; set; }
    
    //foreign key property
    public Guid UserId { get; set; }
}