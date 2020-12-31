using System;

namespace Clima.Contracts.Models
{
    public class ClimateReading
    {
        public ClimateReading()
        {
        }

        public long? ID { get; set; }
        public float? TempC { get; set; }
        public float? BarometricPressurehPa { get; set; }
        public float? RelativeHumdity { get; set; }
        public DateTime timeStamp { get; set;  }
    }
}