using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatgptBlpInsertdata
{
    using System.IO;

    public static class AppConfig
    {
        public static string RootFolder => ConfigurationManager.AppSettings["rootFolder"];

        public static string Pending => Path.Combine(RootFolder, "Pending");
       
        public static string Processed => Path.Combine(RootFolder, "Processed");
        public static string ReadError => Path.Combine(RootFolder, "Error", "ReadError");
        public static string DbError => Path.Combine(RootFolder, "Error", "DbError");

        public static string ConnectionString => ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    }

}
