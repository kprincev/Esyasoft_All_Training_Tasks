
using Microsoft.Data.SqlClient;
using System.Data;
using Aug26Task3MultipleFilesDataStoreInDb.Config;

namespace Aug26Task3MultipleFilesDataStoreInDb.DbService
{
    public class DataBaseService
    {
        public static void InsertIntoDatabase(string json)
        {
            using SqlConnection con =
  new SqlConnection(ConfigReader.ConnectionString);

            con.Open();

            using SqlCommand cmd = new SqlCommand(ConfigReader.SpName, con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Json",   SqlDbType.NVarChar,-1    ).Value = json;

            cmd.ExecuteNonQuery();
        }
    }
}