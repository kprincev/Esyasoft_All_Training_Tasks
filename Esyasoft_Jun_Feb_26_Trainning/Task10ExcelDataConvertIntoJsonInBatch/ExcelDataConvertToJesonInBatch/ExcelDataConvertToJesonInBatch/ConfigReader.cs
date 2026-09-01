using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDataConvertToJesonInBatch
{
    public class ConfigReader
    {
        public static  string outputPath => ConfigurationManager.AppSettings["OutputFolder"];
        public static int batchsize => int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
        public static string filePath => ConfigurationManager.AppSettings["FilePath"];
    }
}
