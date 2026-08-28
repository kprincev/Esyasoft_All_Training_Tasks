using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


namespace Aug26ETTask4MultiTableToMultiFilesCreate
{
    public class ConfigReader
    {
        public static IConfiguration config = null!;

        public static string ConnectionString => config["DbConfig:ConStr"]!;

        public static string SaveLocation => config["DirectoryConfig:SaveLocation"]!;
        public static string SpName => config["SP_Name"];
    }
}
