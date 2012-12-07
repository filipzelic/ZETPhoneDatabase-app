using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseFiller
{
    public class DepartureTimeContext
    {
        public int Id { get; set; }
        public int VoziloId { get; set; }
        public int StanicaId { get; set; }
        public string Time { get; set; }
        public string DayStatus { get; set; }

        public DepartureTimeContext(int voziloId, int stanicaId, string time, string dayStatus)
        {
            VoziloId = voziloId;
            StanicaId = stanicaId;
            Time = time;
            DayStatus = dayStatus;
        }


    }

 
}
