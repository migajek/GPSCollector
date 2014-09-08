using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationModel
{
    [Serializable]
    public class DataReading
    {
        public double Latitude { get; set; }     
        public double Longitude { get; set; }        
        public float Speed { get; set; }        
        public float Bearing { get; set; }        
        public float Accuracy { get; set; }        
        public double GPSAltitude { get; set; }       
        public int GSMSignalStrength { get; set; }        
        public double Pressure { get; set; }
        public int NetworkType { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime? TimeSent { get; set; }
        public DateTime? TimeReceived { get; set; }
    }
}
