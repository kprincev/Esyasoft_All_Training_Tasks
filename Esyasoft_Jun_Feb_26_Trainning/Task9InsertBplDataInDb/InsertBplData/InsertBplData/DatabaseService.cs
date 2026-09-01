using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertBplData
{
   public class DatabaseService
    {
        public static void  Database(List<MeterRecord> records) 
        {
            using (SqlConnection conn = new SqlConnection(ConfigReader.connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("InsertBlpData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    for (int k = 0; k < records.Count; k = k + 4)
                    {


                        Console.WriteLine("Meter id   " + "time interval   " + "Avg voltage" + "KWH     " + "KVAH    " + "Current A");
                        for (int i = 0; i < records[k].Data.Count; i++)
                        {
                            Console.Write($"{records[k].MeterId}   ");
                            cmd.Parameters.AddWithValue("@Meter_Id", records[k].MeterId);

                            DateTime baseTime = records[k].FirstIntervalDateTime;

                            DateTime intervalTime = baseTime.AddMinutes(records[k].interval * i);
                            cmd.Parameters.AddWithValue("@FirstIntervalDateTime", intervalTime);
                            Console.Write($"{intervalTime:dd-MM-yyyy hh:mm tt}   ");
                            string[] units = { "@V", "@kWh", "@kVah", "@A" };
                            int unit = 0;
                            for (int j = k; j < k + 4; j++)
                            {


                                cmd.Parameters.AddWithValue($"{units[unit]}", records[j].Data[i]);
                                Console.Write($"{records[j].Data[i]}   ");
                                unit++;

                            }
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            Console.WriteLine();
                        }
                        Console.WriteLine("---------------------------------------------------------");
                    }
                }

            }
        }

    }
}
