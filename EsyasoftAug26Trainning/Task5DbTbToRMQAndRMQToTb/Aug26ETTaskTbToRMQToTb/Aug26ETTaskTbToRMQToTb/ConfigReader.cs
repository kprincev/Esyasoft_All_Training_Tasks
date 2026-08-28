using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Aug26ETTaskTbToRMQToTb
{
    public class ConfigReader
    {
        public static IConfiguration config = null!;

        public static string ConnectionString => config["DbConfig:ConStr"]!;

        public static string ServiceMode => config["ServiceMode"]!;
        public static string Host => config["HostConfig:Host"];
        public static int Port => int.Parse(config["HostConfig:Port"]);
        public static string UserName => config["HostConfig:UserName"];
        public static string Password => config["HostConfig:Password"];


        public static string SP_Publiser => config["SP_Publiser"];
        public static string SP_Consumer => config["SP_Consumer"];
        public static string UpdateCounter => config["UpdateCounter"];
        public static string queueName => config["QueueName"];
        public static string Url => config["ApiPublisherEndPoint"];
        public static string logPath => config["logFilePath"];

    }
}
