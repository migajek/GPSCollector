using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;

namespace CommunicationModel
{
    public class DataReading
    {
        [SerializeField(10)]
        public double Latitude { get; set; }
        [SerializeField(11)]
        public double Longitude { get; set; }
        [SerializeField(12)]
        public float Speed { get; set; }
        [SerializeField(13)]
        public float Bearing { get; set; }
        [SerializeField(14)]
        public float Accuracy { get; set; }
        [SerializeField(15)]
        public double GPSAltitude { get; set; }
        [SerializeField(16)]
        public double Pressure { get; set; }
        [SerializeField(20)]
        public int GSMSignalStrength { get; set; }              
        [SerializeField(21)]
        public int NetworkType { get; set; }

        [SerializeField(40)]
        public DateTime TimeCreated { get; set; }
        [SerializeField(41)]
        public DateTime? TimeSent { get; set; }
        [SerializeField(42)]
        public DateTime? TimeReceived { get; set; }
    }
}
1