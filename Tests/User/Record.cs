using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Asclepius.User
{
    public class Record
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        //Pedometer stats

        public double Stride { get; set; } //in centimeters
        public int SurfaceGrade { get; set; }
        public int WalkingStepCount { get; set; }
        public int RunningStepCount { get; set; }
        public uint RunTime { get; set; }
        public uint WalkTime { get; set; }
        public int ActivityType { get; set; } //placeholder, default 0 is walking

        //Constructor
        public Record() { }

        public Record(DateTime _start, DateTime _end, double _stride = 0, int _type = 0)
        {
            this.StartDate = _start;
            this.EndDate = _end;
            this.ActivityType = _type;
            this.Stride = _stride;
        }

        //Methods
        public void StartRecord()
        {
            StartDate = DateTime.Now;
        }

        public void StopRecord()
        {
            EndDate = DateTime.Now;
        }

        //Converters

        public double Distance()
        {
            return (WalkingStepCount + RunningStepCount) * Stride / 2;
        }

        public double TimeInMinutes()
        {
            return (EndDate-StartDate).TotalMinutes;
        }

        public double TimeInHours()
        {
            return TimeInMinutes() / 60;
        }

        public double DistanceInMeters()
        {
            return Distance() / 100;
        }

        public double DistanceInKilometers()
        {
            return Distance() / 1000;
        }

        public double DistanceInMiles()
        {
            return DistanceInKilometers() * 0.621371;
        }
        
        public int CompareTo(Record target)
        {
            return StartDate.CompareTo(target.StartDate);
        }
    }
}
