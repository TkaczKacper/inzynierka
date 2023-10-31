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
    public float LongTermStress { get; set; } 
    public float ShorTermStress { get; set; }
    public float StressBalance { get; set; }
    
    // power only
    public float LongTermStressPower { get; set; }
    public float ShortTermStressPower { get; set; }
    public float StressBalancePower { get; set; }
    
    // heart rate only
    public float LongTermStressHr { get; set; }
    public float ShortTermStressHr { get; set; }
    public float StressBalanceHr { get; set; }
    
    // foreign key property
    [JsonIgnore]
    [ForeignKey("User")]
    public virtual Guid UserId { get; set; }
}