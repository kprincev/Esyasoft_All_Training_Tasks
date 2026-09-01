using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelFileReadAndInsertJson
{
    public class ConfigReader

    {
        public static IConfiguration config;
     
        
        public static string PendingFolder => config["DirectoryConfig:PendingFolder"];

        public static string ProcessedFolder => config["DirectoryConfig:ProcessedFolder"];

        public static string ReadErrorFolder => config["DirectoryConfig:ReadErrorFolder"];

        public static string DbErrorFolder => config["DirectoryConfig:DbErrorFolder"];

        public static string ConnectionString => config["DbConfig:ConStr"];
        public static int DbTimeOut=> int.Parse(config["DbConfig:DbTimeOut"]);  
        public static int SleepTime=>int.Parse(config["WorkConfig:SleepTime"]);
        public static int MultiThread => int.Parse(config["WorkConfig:ApplyMT"]);
        public static int MaxThCount => int.Parse(config["WorkConfig:MaxThread"]);
    }
}
