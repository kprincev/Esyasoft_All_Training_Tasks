using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsHoursSummeryData
{
    public class MeterCounter
    {
        public string MSN;
        public DateTime LastProcessedTS;
    }

    public class RawData
    {
        public string ConsumerNumber;
        public string MSN;
        public DateTime TS;
        public decimal Voltage;
        public decimal Current;
        public decimal KW;
        public decimal KVA;
        public decimal KVARh;
    }

    public class GetDataService
    {

        public static List<MeterCounter> GetMeters(SqlConnection con)
        {
            var list = new List<MeterCounter>();

            var cmd = new SqlCommand("SP_GetMeterProcessingCounter", con);
            cmd.CommandType = CommandType.StoredProcedure;

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new MeterCounter
                    {
                        MSN = dr.GetString(0),
                        LastProcessedTS = dr.GetDateTime(1)
                    });
                }
            }

            return list;
        }
        public static List<RawData> GetSinglePhaseData(SqlConnection con, string msn, DateTime lastProcessedTS)
        {
            var list = new List<RawData>();

            DateTime startDate = lastProcessedTS.Date.AddDays(1);
            DateTime endDate = startDate.AddDays(1);


            using (SqlCommand cmd = new SqlCommand("SP_GetSinglePhaseRawData", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MSN", msn);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                using (var dr = cmd.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        list.Add(new RawData
                        {
                            ConsumerNumber = dr.GetString(0),  
                            MSN = dr.GetString(1),             
                            TS = dr.GetDateTime(2),            
                            Voltage = dr.GetDecimal(3),
                            Current = dr.GetDecimal(4),
                            KW = dr.GetDecimal(5),
                            KVA = dr.GetDecimal(6),
                            KVARh = dr.GetDecimal(7)
                        });

                    }
                }
            }

            return list;
        }
        public static List<RawData> GetThreePhaseData( SqlConnection con, string msn,  DateTime lastProcessedTS)
        {
            var list = new List<RawData>();

            DateTime startDate = lastProcessedTS.Date.AddDays(1);
            DateTime endDate = startDate.AddDays(1);



            using (SqlCommand cmd = new SqlCommand("SP_GetThreePhaseRawData", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MSN", msn);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                using (var dr = cmd.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        list.Add(new RawData
                        {
                            ConsumerNumber = dr.GetString(0),
                            MSN = dr.GetString(1),
                            TS = dr.GetDateTime(2),
                            Voltage = dr.GetDecimal(3),
                            Current = dr.GetDecimal(4),
                            KW = dr.GetDecimal(5),
                            KVA = dr.GetDecimal(6),
                            KVARh = dr.GetDecimal(7)
                        });

                    }
                }
            }

            return list;
        }
    }
}
