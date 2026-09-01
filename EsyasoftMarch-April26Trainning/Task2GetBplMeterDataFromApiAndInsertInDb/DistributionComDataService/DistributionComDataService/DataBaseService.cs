using DistributionConsole;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionConsole
{
    public  class DataBaseService
    {
        public static int InsertData(string responseJson)
        {
            int max = 0;

            using (SqlConnection con = new SqlConnection(ConfigReader.conStr))
            {
                con.Open();

                using (SqlCommand insertCmd = new SqlCommand("InsertApiResponse", con))
                {
                    insertCmd.CommandType = CommandType.StoredProcedure;

                    insertCmd.Parameters.AddWithValue("@JsonData", responseJson);
                    insertCmd.Parameters.AddWithValue("@MeterType", ConfigReader.meterType);
                    insertCmd.Parameters.AddWithValue("@BLPYear", ConfigReader.year);


                    max = Convert.ToInt32(insertCmd.ExecuteScalar());
                }

                con.Close();
            }
            return max;
        }
    }
}
