using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace server.Models.Profile;

public class TrainingLoad
{
    [JsonIgnore]
    public long Id { get; set; }
    public DateOnly Date { get; set; }
    public int TrainingStressScore { get; set; }
    public int TrainingImpulse { get; set; }
    
    // power & hr data mixed
    public int LongTermStress { get; set; } 
    public int ShortTermStress { get; set; }
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
    [JsonIgnore]
    [ForeignKey("User")]
    public virtual Guid UserId { get; set; }
}