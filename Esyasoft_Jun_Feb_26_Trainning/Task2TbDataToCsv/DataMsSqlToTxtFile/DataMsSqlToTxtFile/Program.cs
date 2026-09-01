using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMsSqlToTxtFile
{
    public class DatabaseConfig
    {
        public string ConnectionString;
        public DatabaseConfig()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
        }

    }
    public class DataExporter
    {
        public void ExportData(string filepath)
        {
            try
            {
                DatabaseConfig dbConfig = new DatabaseConfig();
                using (SqlConnection conn = new SqlConnection(dbConfig.ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("select * from student;", conn))
                    {
                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath))
                            {
                                while (rd.Read())
                                {
                                    sw.WriteLine($"Stu_id={rd["Stu_id"]},Gender={rd["Gender"]},Age={rd["Age"]},Email={rd["Email"]},Address={rd["Addresh"]}");
                                }
                            }
                        }
                    }
                    Console.WriteLine("Data exported successfully to " + filepath);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            DataExporter exporter = new DataExporter();
            exporter.ExportData("D:\\Esyasoft Trainning Tasks\\Esyasoft_Jun_Feb_26_Trainning\\Task2TbDataToCsv\\OutputCsvFile\\output.txt");
        }
    }
}
