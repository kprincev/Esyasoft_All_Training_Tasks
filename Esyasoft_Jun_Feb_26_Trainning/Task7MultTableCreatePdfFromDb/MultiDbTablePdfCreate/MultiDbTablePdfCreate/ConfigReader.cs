using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDbTablePdfCreate
{
    internal class ConfigReader
    {
        public static string connectionString =>
           ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        public static string pdfPath=>
           ConfigurationManager.AppSettings["PdfPath"];
        public static string logoPath =>ConfigurationManager.AppSettings["LogoPath"];
        public static string headerText =>ConfigurationManager.AppSettings["HeaderText"];
        public static int tableCount=>int.Parse(ConfigurationManager.AppSettings["TableCount"]);
        public static int maxThreads =>
    int.Parse(ConfigurationManager.AppSettings["MaxThreads"]);


    }
}
