using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InsertDataInMSSql
{
    public class Services
    {
        public static void InsertDataInMsql(string name, string Email, string phone, int salary)
        {
            ;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigReader.connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("InsertDataInEmp", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@salary", salary);
                        cmd.ExecuteNonQuery();
                    }

                }
                Console.WriteLine("Data Inserted Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static  void ExportStoredProcToCsv()
        {
            
            string fileName = "SQL_Export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

           
            string fullPath = Path.Combine(ConfigReader.folderPath, fileName);
          
            if (!Directory.Exists(ConfigReader.folderPath))
            {
                Directory.CreateDirectory(ConfigReader.folderPath);
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigReader.connectionString))
                {
                   
                    SqlCommand cmd = new SqlCommand("extrectdata", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        using (StreamWriter sw = new StreamWriter(fullPath, false, Encoding.UTF8))
                        {
                      
                            string[] columnNames = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columnNames[i] = reader.GetName(i);
                            }
                            sw.WriteLine(string.Join(",", columnNames));

                            while (reader.Read())
                            {
                                string[] rowValues = new string[reader.FieldCount];
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string value = reader[i].ToString();

                                    if (value.Contains(",") || value.Contains("\""))
                                        value = $"\"{value.Replace("\"", "\"\"")}\"";

                                    rowValues[i] = value;
                                }
                                sw.WriteLine(string.Join(",", rowValues));
                            }
                        }
                    }
                }
                Console.WriteLine("Success: SP data CSV mein export ho gaya!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
