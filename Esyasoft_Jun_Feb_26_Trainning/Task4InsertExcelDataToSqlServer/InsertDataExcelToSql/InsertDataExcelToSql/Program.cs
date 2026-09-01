using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using ExcelDataReader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertDataExcelToSql
{
    class DatabaseConfig
    {
        public string connectionString;
        public DatabaseConfig()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;

        }
    }
    class DataInserter
    {
        public void InsertDataFromExcel()
        {

            DatabaseConfig dbConfig = new DatabaseConfig();
            try
            {


                using (SqlConnection conn = new SqlConnection(dbConfig.connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("InsertBook", conn))
                    {
                        using (var stream = File.Open(@"C:\Users\Prince\Downloads\heyy.xlsx", FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }
                                });
                                DataTable dt = ds.Tables[0];
                                cmd.CommandType = CommandType.StoredProcedure;
                                foreach (DataRow row in dt.Rows)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@book_name", row["book"]);
                                    cmd.ExecuteNonQuery();
                                   
                                }
                                Console.WriteLine("Data insert successful");

                            }
                        }


                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
    }
        internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseConfig ob=new DatabaseConfig();
            DataInserter inserter = new DataInserter();
            inserter.InsertDataFromExcel();


        }
    }
}
