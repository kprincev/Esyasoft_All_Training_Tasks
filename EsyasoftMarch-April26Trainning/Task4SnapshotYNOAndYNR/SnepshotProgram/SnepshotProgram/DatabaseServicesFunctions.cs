using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnepshotProgram
{
    public class DatabaseServicesFunctions
    {
        public static void insertDestination( string json, SqlConnection con)
        {
            using (var cmd = new SqlCommand("InsertDestinationJson", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@json", json);

                cmd.ExecuteNonQuery();
            }
        }
        public static void UpdateBatchCounter(int lastId, SqlConnection con)
        {
            using (var cmd = new SqlCommand("updatebatchcounter", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@lastid", lastId);
                cmd.ExecuteNonQuery();
            }
        }
        public static void YnoYnrInsertDelete(List<DataRow>ynoToAdd, List<DataRow> ynrToAdd,List<int> ynoToDelete,List<int > ynrToDelete, SqlConnection con)
        {
            using (var cmd = new SqlCommand("ProcessYnoYnrJson", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ynoInsertJson",
                    ynoToAdd.Any() ? HelperFunction.ConvertToJson(ynoToAdd) : (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ynrInsertJson",
                    ynrToAdd.Any() ? HelperFunction.ConvertToJson(ynrToAdd) : (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ynoDeleteJson",
                    ynoToDelete.Any() ? JsonConvert.SerializeObject(ynoToDelete) : (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ynrDeleteJson",
                    ynrToDelete.Any() ? JsonConvert.SerializeObject(ynrToDelete) : (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
        public static DataTable GetBatch(string connectionString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetSourceBatch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(dt);
                    }

                }
            }
            return dt;
        }
        public static List<DataRow> ExecuteSPToList(string spName, string json, SqlConnection con)
        {
            var dt = new DataTable();

            using (var cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@json", json);

                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt.AsEnumerable().ToList();
        }
    }
}
