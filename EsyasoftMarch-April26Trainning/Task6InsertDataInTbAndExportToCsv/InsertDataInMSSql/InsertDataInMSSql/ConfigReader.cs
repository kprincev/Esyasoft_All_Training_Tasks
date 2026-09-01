using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertDataInMSSql
{
    public  class ConfigReader
    {
        public static string connectionString=> System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        public static string  folderPath => System.Configuration.ConfigurationManager.AppSettings["folderPath"];
        public static string flag=> System.Configuration.ConfigurationManager.AppSettings["flag"];
    }
}
