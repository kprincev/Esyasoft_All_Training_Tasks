using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Task6ReadXmlFileRealWorkingDataToJsonInDb
{
    public class DataBaseService
    {
        public void  InsertJsonIPData(string json)
        {
            using (SqlConnection connection = new SqlConnection(ConfigReader.ConnectionString))
            using (SqlCommand command = new SqlCommand(ConfigReader.IP_DataInsertSp, connection))
            {
                connection.Open();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@json", json);
                command.ExecuteNonQuery();

            }
        }
    }
}
