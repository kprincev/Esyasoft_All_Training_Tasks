using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Task6ReadXmlFileRealWorkingDataToJsonInDb
{
    public class ConfigReader
    {
        public static IConfiguration config = null!;

        // Configuration Properties
        public static string ConnectionString => config["DbConfig:ConStr"]!;
        public static string LngFolderPath => config["LngFolderPath"]!;
        public static string IP_DataInsertSp => config["SP:IP_DataInsertSp"]!;
        public static string JsonFileCreateMode => config["JsonFileCreateMode"]!;
        public static int ServiceMode => int.Parse(config["ServiceMode:Mode"]!);

        // Main Directory Paths
        public static string PendingFolder => Path.Combine(LngFolderPath, "Pending");
        public static string ProcessedFolder => Path.Combine(LngFolderPath, "Processed");
        public static string JsonOutputFolder => Path.Combine(LngFolderPath, "JsonOutput");
        public static string DBErrorFilesFolder => Path.Combine(LngFolderPath, "DBErrorFiles");
        public static string ReadErrorFilesFolder => Path.Combine(LngFolderPath, "ReadErrorFiles");

        // Sub-Directory Paths
        public static string DbErrorJsonFolder => Path.Combine(JsonOutputFolder, "DbErrorJson");
        public static string ProcessedJsonFolder => Path.Combine(JsonOutputFolder, "ProcessedJson");
        public static int FileBatchSize => int.Parse(config["ProcessingConfig:FileBatchSize"]!);

        public static int MaxThread => int.TryParse(config["ProcessingConfig:MaxThreadCount"], out int threads) ? threads : 1;

        // Dynamic Folder Ensure Method
        public static void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(PendingFolder);
            Directory.CreateDirectory(ProcessedFolder);
            Directory.CreateDirectory(JsonOutputFolder);
            Directory.CreateDirectory(DBErrorFilesFolder);
            Directory.CreateDirectory(ReadErrorFilesFolder);
            Directory.CreateDirectory(DbErrorJsonFolder);
            Directory.CreateDirectory(ProcessedJsonFolder);
        }
        public static string GetTargetFolderPath()
        {
            int currentMode = ServiceMode;

            // Appsettings check
            string payloadName = config[$"ServiceMode:PayloadModes:{currentMode}"] ?? "ALL";

            if (payloadName.ToUpper() == "ALL" || currentMode == 0)
            {
                return PendingFolder; //In Every Case ALL root folder
            }
            return Path.Combine(PendingFolder, payloadName);
        }
    }
}