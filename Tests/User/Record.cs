using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.User
{
    public struct Record
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        //Pedometer stats
        public int WalkingSteps { get; set; }
        public int RunningSteps { get; set; }
        public double StepFreq { get; set; }
        public double Distance { get; set; } //in km
        public long WalkTime { get; set; } //in seconds
        public int SurfaceGrade { get; set; }

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
        public double TimeInMinutes()
        {
            return WalkTime / 60;
        }

        public double TimeInHours()
        {
            return TimeInMinutes() / 60;
        }

        public double DistanceInMeters()
        {
            return Distance * 1000;
        }

        public double DistanceInMiles()
        {
            return Distance * 0.621371;
        }
        
        public int CompareTo(Record target)
        {
            return StartDate.CompareTo(target.StartDate);
        }
    }
}
