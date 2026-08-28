
using Microsoft.Extensions.Configuration;
using Aug26Task3MultipleFilesDataStoreInDb.Config;
using Aug26Task3MultipleFilesDataStoreInDb.Service;
using Aug26Task3MultipleFilesDataStoreInDb.ServiceBridge;

class Program
{
    public static void Main(string[] args)
    {
        ConfigReader.config = new ConfigurationBuilder() .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(   "appsettings.json",  optional: false, reloadOnChange: true)
            .Build();


        Console.WriteLine("===== Service Is Live =====");
        Directory.CreateDirectory(ConfigReader.PendingFolder);
        Directory.CreateDirectory(ConfigReader.ProcessedFolder);
        Directory.CreateDirectory(ConfigReader.ErrorFolder);
        switch (ConfigReader.Flag)
        {
            case 1:
                Console.WriteLine("Processing JSON Files");

                ServiceBridge.ProcessFiles(    ConfigReader.PendingFolder,  "*.json",  Services.JsonToDataTable  );
                break;


            case 2:

                Console.WriteLine("Processing CSV Files");
                ServiceBridge.ProcessFiles(  ConfigReader.PendingFolder,"*.csv", Services.CsvToDataTable );
                break;


            case 3:

                Console.WriteLine("Processing XML Files");
                ServiceBridge.ProcessFiles( ConfigReader.PendingFolder, "*.xml", Services.XmlToDataTable );
                break;


            case 4:

                Console.WriteLine("Processing Excel Files");
                ServiceBridge.ProcessFiles(ConfigReader.PendingFolder, "*.xlsx",Services.ExcelToDataTable  );
                break;


            case 5:

                Console.WriteLine("Processing All Files");

                ServiceBridge.ProcessFiles( ConfigReader.PendingFolder, "*.json",Services.JsonToDataTable);

                ServiceBridge.ProcessFiles(  ConfigReader.PendingFolder, "*.csv",Services.CsvToDataTable);

                ServiceBridge.ProcessFiles(ConfigReader.PendingFolder, "*.xml", Services.XmlToDataTable);

                ServiceBridge.ProcessFiles( ConfigReader.PendingFolder, "*.xlsx", Services.ExcelToDataTable );
                break;


            default:
                Console.WriteLine("Invalid Flag");
                break;
        }
        Console.WriteLine("Service Completed");
    }
}