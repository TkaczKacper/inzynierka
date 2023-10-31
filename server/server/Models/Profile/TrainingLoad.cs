using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Profile;

public class TrainingLoad
{
    public long Id { get; set; }
    public DateOnly Date { get; set; }
    public int TrainingStressScore { get; set; }
    public int TrainingImpulse { get; set; }
    
    // power & hr data mixed
    public int LongTermStress { get; set; } 
    public int ShorTermStress { get; set; }
    public int StressBalance { get; set; }
    
    // power only
    public int LongTermStressPower { get; set; }
    public int ShortTermStressPower { get; set; }
    public int StressBalancePower { get; set; }
    
    // heart rate only
    public int LongTermStressHr { get; set; }
    public int ShortTermStressHr { get; set; }
    public int StressBalanceHr { get; set; }
    
    // foreign key property
    [ForeignKey("User")]
    public virtual Guid UserId { get; set; }
}