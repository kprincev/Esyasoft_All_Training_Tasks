using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace WeatherApiDataInserter.ModelClass
{
    public class WeatherResponse
    {
        public string name { get; set; }
        public Main main { get; set; }
        public long dt { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
    }
    
}
