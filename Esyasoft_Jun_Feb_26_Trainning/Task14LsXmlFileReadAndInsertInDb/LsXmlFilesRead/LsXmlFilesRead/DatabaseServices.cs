using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsXmlFilesRead
{
    public class DatabaseServices
    {
        public static void InsertJsonToDb(string json, string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Insert_LS_FromJson", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@JsonData", SqlDbType.NVarChar).Value = json;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
