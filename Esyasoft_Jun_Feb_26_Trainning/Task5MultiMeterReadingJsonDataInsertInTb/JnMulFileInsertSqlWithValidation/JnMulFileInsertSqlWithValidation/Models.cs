using System;
using System.Collections.Generic;

namespace MeterBatchProcessor.Models
{
    public class MeterReading
    {
        public string MeterId { get; set; }
        public DateTime ReadingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Breakdown> Breakdown { get; set; }
    }
    public class Breakdown
    {
        public string Parameter { get; set; }
        public decimal Value { get; set; }
    }
}
