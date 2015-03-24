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
        public double Weight { get; set; } //user weight in kg
        public double Height { get; set; } //user height in meters

        //Device stats
        public int PulseRate { get; set; }
        public double Temperature { get; set; } //in Celcius

        //Pedometer stats
        public int WalkingSteps { get; set; }
        public int RunningSteps { get; set; }
        public double StepFreq { get; set; }
        public double Distance { get; set; } //in km
        public long WalkTime { get; set; } //in seconds
        public int SurfaceGrade { get; set; }

        //Converters
        public double WeightInLbs()
        {
            return Weight * 2.20462;
        }

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

        public double TempInFahrenheit()
        {
            return Temperature * 1.8 + 32;
        }

        public int CompareTo(Record target)
        {
            return StartDate.CompareTo(target.StartDate);
        }
    }
}
