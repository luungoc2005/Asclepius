using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.User
{
    public class Snapshot
    {
        public DateTime DateTaken { get; set; }

        public double Weight { get; set; } //user weight in kg
        public double Height { get; set; } //user height in meters
        
        //Device stats
        public int PulseRate { get; set; }
        public double Temperature { get; set; } //in Celcius


        public double WeightInLbs()
        {
            return Weight * 2.20462;
        }

        public double TempInFahrenheit()
        {
            return Temperature * 1.8 + 32;
        }
    }
}
