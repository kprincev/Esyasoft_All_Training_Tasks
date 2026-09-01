using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexFindInDbPrint
{
  public class DataBaseService
  {
        private string connectionString;
        public DataBaseService()
        {
             connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        }
        public string DataBaseindex(int i) 
        {
            string result;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("bitoutput", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@index", i + 1);
                    SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.NVarChar, 100);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();


                    result = outputParam.Value.ToString();
                  

                }
            }
            return result;

        }
    }
}
