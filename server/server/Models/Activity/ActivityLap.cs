using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.Activity
{
    public class ActivityLap
    {
        public long ID { get; set; }
        public int ElapsedTime { get; set; }
        public int MovingTime { get; set; }
        public DateTime StartDate { get; set; }
        public float Distance { get; set; }
        public int StartIdx { get; set; }
        public int EndIdx { get; set; }
        public int LapIndex { get; set; }
        public float TotalElevationGain { get; set; }
        public float AvgSpeed { get; set; }
        public float MaxSpeed { get; set; }
        public float AvgWatts { get; set; }
        public float AvgCadence { get; set; }
        public float AvgHeartRate { get; set; }
        public float MaxHeartRate { get; set; }
        
        
        //foreign key property
        [ForeignKey("Activity")]
        public virtual long ActivityId { get; set; }
    }
}
