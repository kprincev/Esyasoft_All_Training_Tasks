using Microsoft.Data.SqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aug26ETTaskTbToRMQToTb
{
    public class DatabaseService
    {
        public static int lastId;
        public static string GetDataForPuplisher()
        {
            string json;

            Log.Information("Try To Make Connection To Db");
            using (SqlConnection connection = new SqlConnection(ConfigReader.ConnectionString))
            {
                Log.Information("Db Connection Succssfull");

                using (SqlCommand command = new SqlCommand(ConfigReader.SP_Publiser, connection))
                {
                    
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    connection.Open();
                    Log.Information("Connection Open Successlly ");
                    object result = command.ExecuteScalar();
                    Log.Information("Resive Data From db");
                    Log.Information("Check Data Empty or Avaliable ");
                    if (result == null || result == DBNull.Value)
                    {
                        Log.Information("Data is empty");
                        return "";
                    }
                    Log.Information("Data is not empty");
                    json = result.ToString();

                    Log.Information("Data is object to json string convert");
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        Log.Information("Parse json string to json object ");

                        JsonElement lastObject = doc.RootElement.EnumerateArray().Last();

                        lastId = lastObject.GetProperty("CustomerID").GetInt32();
                        Log.Information($"Get Last id of customer  {lastId}");


                    }
                }
            }
            return json;
        }
        public static void UpDateCounter()
        {
            using (SqlConnection connection = new SqlConnection(ConfigReader.ConnectionString))
            using (SqlCommand command = new SqlCommand(ConfigReader.UpdateCounter, connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                connection.Open();
                command.Parameters.AddWithValue("@lastid", lastId);

                command.ExecuteNonQuery();
                
            }
        }
        public static async Task InsertDataForConsumer(string json)
        {
            using (SqlConnection connection = new SqlConnection(ConfigReader.ConnectionString)) 
            using (SqlCommand command = new SqlCommand(ConfigReader.SP_Consumer, connection))
            {
                connection.Open();
                command.CommandType=System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@json", json);
                command.ExecuteNonQuery();
        
            }
        }

    }
}
