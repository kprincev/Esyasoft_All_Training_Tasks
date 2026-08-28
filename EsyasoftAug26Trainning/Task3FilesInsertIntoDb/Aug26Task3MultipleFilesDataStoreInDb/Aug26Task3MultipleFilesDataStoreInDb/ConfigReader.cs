
using Microsoft.Extensions.Configuration;

namespace Aug26Task3MultipleFilesDataStoreInDb.Config
{
    public class ConfigReader
    {
        public static IConfiguration config = null!;

        public static string ConnectionString =>  config["DbConfig:ConStr"]!;

        public static string PendingFolder => config["DirectoryConfig:PendingFolder"]!;

        public static string ProcessedFolder => config["DirectoryConfig:ProcessedFolder"]!;

        public static string ErrorFolder =>config["DirectoryConfig:ErrorFolder"]!;

        public static int Flag =>int.Parse(config["Flag"]!);
        public static string SpName => config["SP_Name"];
    }
}