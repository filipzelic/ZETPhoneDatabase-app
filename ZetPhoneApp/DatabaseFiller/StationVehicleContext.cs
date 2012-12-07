using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseFiller
{
    /// <summary>
    /// Info o 
    /// </summary>
    public class StationVehicleContext
    {
        public int Id { get; set; }
        public int StanicaId { get; set; }
        public int VoziloId { get; set; }
        public int PolazisnaStanicaId { get; set; }
        public int TimeOffset { get; set; }


        public StationVehicleContext(int stanicaId, int voziloId, int polazisnaStanicaId, int timeOffset)
        {
            StanicaId = stanicaId;
            VoziloId = voziloId;
            PolazisnaStanicaId = polazisnaStanicaId;
            TimeOffset = timeOffset;
        }
    }
}
