using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsHoursSummeryData
{
    public class ProcessData
    {
        public static void ProcessMeter(SqlConnection con, MeterCounter meter)
        {
            Console.WriteLine($"Processing Meter: {meter.MSN}");

            var rawData = GetDataService.GetSinglePhaseData(con, meter.MSN, meter.LastProcessedTS);
            string meterType = "SINGLE_PHASE";

            if (!rawData.Any())
            {
                rawData = GetDataService.GetThreePhaseData(con, meter.MSN, meter.LastProcessedTS);
                meterType = "THREE_PHASE";
            }

            if (!rawData.Any())
            {
                Console.WriteLine("No complete day data available.");
                return;
            }

       
            var hourlyGroups = rawData .GroupBy(x => new DateTime( x.TS.Year,x.TS.Month,x.TS.Day,x.TS.Hour,0,0)).OrderBy(x => x.Key).ToList();

            Console.WriteLine($"Hour Count: {hourlyGroups.Count}");

            if (hourlyGroups.Count != 24)
            {
                Console.WriteLine("Day not complete. Skipping.");
                return;
            }

            foreach (var hourGroup in hourlyGroups)
            {
                PutDataService.InsertHourly(con, hourGroup, meterType);
            }

           
            DateTime newCounterDate = meter.LastProcessedTS.Date.AddDays(1);

            PutDataService.UpdateCounter(con, meter.MSN, newCounterDate);
        }
    }
}
