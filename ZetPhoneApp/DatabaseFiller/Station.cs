using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseFiller
{
    public class Station
    {
        public int Id { get; set; }
        public float Altitude { get; set; }
        public float Longitude { get; set; }
        public string Name { get; set; }

        public Station(float altitude, float longitude, string name)
        {
            Altitude = altitude;
            Longitude = longitude;
            Name = name;
        }

    }
}
