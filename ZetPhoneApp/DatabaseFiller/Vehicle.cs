using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseFiller
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int LineNumber { get; set; }
        public string Type { get; set; }

        public Vehicle(int lineNumber, string type)
        {
            LineNumber = lineNumber;
            Type = type;
        }

    }
}
