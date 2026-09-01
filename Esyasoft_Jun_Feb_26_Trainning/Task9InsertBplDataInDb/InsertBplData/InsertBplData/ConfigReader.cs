using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertBplData
{
    public static class ConfigReader
    {
        public static string RootFolder => ConfigurationManager.AppSettings["rootFolder"];

        public static string Pending => Path.Combine(RootFolder, "Pending");
        public static string Processing => Path.Combine(RootFolder, "Processing");
        public static string Processed => Path.Combine(RootFolder, "Processed");

        public static string SyntaxError => Path.Combine(RootFolder, "Error", "SyntaxError");
        public static string ReadError => Path.Combine(RootFolder, "Error", "ReadError");
        public static string DbError => Path.Combine(RootFolder, "Error", "DbError");
        public static string  connectionString => ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        
    }
}
