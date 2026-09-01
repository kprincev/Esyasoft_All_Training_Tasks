using DocumentFormat.OpenXml.Office.Word;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertAnd
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
        
        
       
        public void DataExportToExcel()
        {
            DataTable dt = new DataTable();
            DatabaseConfig dbconfig = new DatabaseConfig();
            try
            {
                using (SqlConnection conn = new SqlConnection(dbconfig.connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Select * from student", conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
                using (var wb = new ClosedXML.Excel.XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Students");

                    wb.SaveAs(@"D:\Esyasoft Trainning Tasks\Esyasoft_Jun_Feb_26_Trainning\Task1TbDataToExcelFile\OuputExcelFile\Student.xlsx");
                    Console.WriteLine("Data Exported Successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }



        internal class Program
        {
            static void Main(string[] args)
            {
                DataInserter inserter = new DataInserter();
                 
                inserter.DataExportToExcel();
                
            }
        }
    }
}
