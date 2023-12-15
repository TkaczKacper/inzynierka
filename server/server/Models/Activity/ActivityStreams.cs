using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Activity;

public class ActivityStreams
{
    public long Id { get; set; }
    public List<int>? TimeStream { get; set; }
    public List<float>? Distance { get; set; }
    public List<float>? Velocity { get; set; }
    public List<int>? Watts { get; set; }
    public List<int>? Cadence { get; set; }
    public List<int>? HeartRate { get; set; }
    public List<int>? Temperature { get; set; }
    public List<float>? Altitude { get; set; }
    public List<float>? GradeSmooth { get; set; }
    public List<bool>? HaveData { get; set; }
    public List<double>? Lat { get; set; } 
    public List<double>? Lng { get; set; }
    
    //foreign key property
    [ForeignKey("Activity")]
    public virtual long ActivityId { get; set; }
}