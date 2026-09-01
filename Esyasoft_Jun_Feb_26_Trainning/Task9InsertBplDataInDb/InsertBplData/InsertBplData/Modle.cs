using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertBplData
{
    public class MeterRecord
    {
        public string MeterId { get; set; }
        public string Units { get; set; }
        public DateTime FirstIntervalDateTime { get; set; }
        public List<decimal> Data { get; set; }
        public int interval { get; set; }
    }

}
