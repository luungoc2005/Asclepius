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

        [XmlIgnore]
        public Lumia.Sense.StepCounterReading stepReading { get; set; }

        public double Stride { get; set; } //in centimeters
        public int SurfaceGrade { get; set; }

        public double Distance()
        {
            return (stepReading.WalkingStepCount + stepReading.RunningStepCount) * Stride / 2;
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
