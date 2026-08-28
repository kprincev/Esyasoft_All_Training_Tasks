using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Task6ReadXmlFileRealWorkingDataToJsonInDb;
using System.Security.Cryptography.X509Certificates;

class Program
{
    public static void Main()
    {
        // 1. Appsettings Configuration Load
        ConfigReader.config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

        ConfigReader.EnsureDirectoriesExist();
        string targetFolder = ConfigReader.GetTargetFolderPath();

        Console.WriteLine($"[Service Engine Started] Target Directory: {targetFolder}");

        if (!Directory.Exists(targetFolder))
        {
            Console.WriteLine($"Directory does not exist: {targetFolder}");
            return;
        }
        TaskService taskService = new TaskService();
        taskService.ProcessFiles(targetFolder);

    }
       
}
