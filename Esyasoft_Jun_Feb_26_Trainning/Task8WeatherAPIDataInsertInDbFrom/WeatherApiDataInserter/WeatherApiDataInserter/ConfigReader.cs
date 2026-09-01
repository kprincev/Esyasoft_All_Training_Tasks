using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace WeatherApiDataInserter
{
    internal class ConfigReader
    {
        public static string connectionString =>
           ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        public static string apikeyy =>    ConfigurationManager.AppSettings["Apikey"];
       
    }
}
