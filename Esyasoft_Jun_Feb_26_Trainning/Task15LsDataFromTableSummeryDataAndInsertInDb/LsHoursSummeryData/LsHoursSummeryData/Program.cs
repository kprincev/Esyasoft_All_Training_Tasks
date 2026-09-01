using LsHoursSummeryData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


class Program
{
    static string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    static void Main()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();

            var meters = GetDataService.GetMeters(con);

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = int.Parse(ConfigurationManager.AppSettings["threadcount"]) 
            };

            Parallel.ForEach(meters, options, meter =>
            {
                using (SqlConnection threadCon = new SqlConnection(conStr))
                {
                    threadCon.Open();
                    ProcessData.ProcessMeter(threadCon, meter);
                }
            });

        }

        Console.WriteLine("Batch Completed");
    }
}

