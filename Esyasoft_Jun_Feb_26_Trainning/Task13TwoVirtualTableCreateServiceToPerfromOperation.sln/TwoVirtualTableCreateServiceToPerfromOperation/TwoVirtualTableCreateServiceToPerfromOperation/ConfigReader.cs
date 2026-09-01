using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TwoVirtualTableCreateServiceToPerfromOperation
{
   public class ConfigReader
    {
        public static string  destinationConnStr => ConfigurationManager.ConnectionStrings["destdb"].ConnectionString;
        public static string sourceConnStr => ConfigurationManager.ConnectionStrings["sourcedb"].ConnectionString;
    }
}
