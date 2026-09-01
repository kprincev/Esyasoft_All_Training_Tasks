using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace ExcelFileReadAndInsertJson
{
    public class DatabaseService
    {

        public static void InsertIntoDatabase(string fileName, string json)
        {
            using (SqlConnection con = new SqlConnection(ConfigReader.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertWorkBookData", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@filename", fileName);
                    cmd.Parameters.AddWithValue("@json", json);

                    con.Open();
                    cmd.CommandTimeout = (int)TimeSpan.FromMilliseconds(ConfigReader.DbTimeOut).TotalSeconds;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
