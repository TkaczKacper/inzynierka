namespace server.Models.Profile.Summary;

public class ProfileMonthlySummary
{
    public long Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public float TotalDistance { get; set; }
    public float TotalElevationGain { get; set; }
    public float TotalCalories { get; set; }
    public int TotalMovingTime { get; set; }
    public int TotalElapsedTime { get; set; }
    public double TrainingLoad { get; set; }
    
    //foreign key property
    public Guid UserId { get; set; } 
}