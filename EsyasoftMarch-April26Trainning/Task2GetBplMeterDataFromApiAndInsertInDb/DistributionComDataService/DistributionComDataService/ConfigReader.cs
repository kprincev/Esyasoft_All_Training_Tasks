using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionConsole
{
    public class ConfigReader
    {
        public static string conStr => ConfigurationManager
               .ConnectionStrings["db"].ConnectionString;

        public static string meterType => ConfigurationManager.AppSettings["MeterType"];
        public static string year => ConfigurationManager.AppSettings["BLPYear"];
        public static string apiUrl => ConfigurationManager.AppSettings["ApiUrl"];
        public static string UserPassword => ConfigurationManager.AppSettings["UserPassword"];
        public static int Dtime => int.Parse(ConfigurationManager.AppSettings["DelayInSeconds"]);
    }
}
