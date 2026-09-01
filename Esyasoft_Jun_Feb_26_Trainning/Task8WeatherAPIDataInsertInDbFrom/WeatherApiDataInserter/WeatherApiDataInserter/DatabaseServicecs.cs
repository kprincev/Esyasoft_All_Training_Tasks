using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApiDataInserter
{
    public class DatabaseServicecs
    {
        public static void InsertWeather(string city, double temp, DateTime date)
        {


            using (SqlConnection con = new SqlConnection(ConfigReader.connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("InsertWeatherdata", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@City", city);
                    cmd.Parameters.AddWithValue("@Tem", temp);
                    cmd.Parameters.AddWithValue("@WD", date);
                    cmd.ExecuteNonQuery();


                }
            }



        }
    }
}
