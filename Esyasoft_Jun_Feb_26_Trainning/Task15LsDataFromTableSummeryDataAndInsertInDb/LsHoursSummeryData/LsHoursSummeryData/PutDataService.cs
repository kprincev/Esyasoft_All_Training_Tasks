using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsHoursSummeryData
{
    public class PutDataService
    {
       public static void InsertHourly(SqlConnection con, IGrouping<DateTime, RawData> group, string meterType)
        {
            using (SqlCommand cmd = new SqlCommand("SP_InsertMeterHourlyTarget", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ConsumerNumber",
     group.First().ConsumerNumber);

                cmd.Parameters.AddWithValue("@MSN", group.First().MSN);
                cmd.Parameters.AddWithValue("@MeterType", meterType);
                cmd.Parameters.AddWithValue("@TS", group.Key);

                cmd.Parameters.AddWithValue("@Avg_V", group.Average(x => x.Voltage));
                cmd.Parameters.AddWithValue("@Avg_I", group.Average(x => x.Current));
                cmd.Parameters.AddWithValue("@Avg_KW", group.Average(x => x.KW));
                cmd.Parameters.AddWithValue("@Avg_KVA", group.Average(x => x.KVA));
                cmd.Parameters.AddWithValue("@Avg_KVARh", group.Average(x => x.KVARh));

                cmd.Parameters.AddWithValue("@Sum_V", group.Sum(x => x.Voltage));
                cmd.Parameters.AddWithValue("@Sum_I", group.Sum(x => x.Current));
                cmd.Parameters.AddWithValue("@Sum_KW", group.Sum(x => x.KW));
                cmd.Parameters.AddWithValue("@Sum_KVA", group.Sum(x => x.KVA));
                cmd.Parameters.AddWithValue("@Sum_KVARh", group.Sum(x => x.KVARh));

                cmd.ExecuteNonQuery();
            }
        }
       public  static void UpdateCounter(SqlConnection con, string msn, DateTime maxTS)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SP_UpdateMeterProcessingCounter", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TS", maxTS);
                cmd.Parameters.AddWithValue("@MSN", msn);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
