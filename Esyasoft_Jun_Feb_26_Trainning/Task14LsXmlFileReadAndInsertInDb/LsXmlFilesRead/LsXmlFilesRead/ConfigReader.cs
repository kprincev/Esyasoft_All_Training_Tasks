using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsXmlFilesRead
{
  public class ConfigReader
    {
   
    
        public static string Pending => ConfigurationManager.AppSettings["Pending"];
        public static string Processed => ConfigurationManager.AppSettings["Processed"];
        public static string ReadError => ConfigurationManager.AppSettings["ReadError"];

        public static string constr => ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    }
}
