using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadToFtpServer
{
    public class ConfigReader
    {
        public static IConfiguration config;
        public static string  ProcessedFolder => config["DirectoryConfig:Processed"];
        public static string PendingFolder => config["DirectoryConfig:Pending"];
        public static int SleepTime => int.Parse(config["WorkConfig:SleepTime"]);
        public static int MultiThread => int.Parse(config["WorkConfig:ApplyMT"]);
        public static int MaxThCount => int.Parse(config["WorkConfig:MaxThread"]);
        public static string Host=> config["FTPConfig:Host"];
        public static string FtpUserName => config["FTPConfig:Username"];
        public static string FtpPassword => config["FTPConfig:Password"];
        
        public static int FtpPort => int.Parse(config["FTPConfig:FtpPort"]);
        public static int BatchSize => int.Parse(config["WorkConfig:BatchSize"]);
        public static int PerFolderBatch => int.Parse(config["WorkConfig:PerFolderBatch"]);
        public static string AllowedExtensions => config["WorkConfig:AllowedExtensions"];


    }
}
