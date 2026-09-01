using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Bibliography;
using ExcelFileReadAndInsertJson;
using System.Threading.Tasks;
class Program
{

    public static async Task Main()
    {
       
       
        string projectPath = Directory.GetParent(AppContext.BaseDirectory)
                                  .Parent.Parent.Parent.FullName;


        ConfigReader.config = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Console.WriteLine("===============================Service Is Live===================");

        Directory.CreateDirectory(ConfigReader.PendingFolder);
        Directory.CreateDirectory(ConfigReader.ProcessedFolder);
        Directory.CreateDirectory(ConfigReader.ReadErrorFolder);
        Directory.CreateDirectory(ConfigReader.DbErrorFolder);

        while (true)
        {
            try
            {
                Console.WriteLine($"[1.Step ]Checking for files in {ConfigReader.PendingFolder}...");
                List<FileInfo> files = new DirectoryInfo(ConfigReader.PendingFolder)
                .GetFiles("*.xlsx")
                .OrderBy(f => f.CreationTime)
                .ToList();

                if (files.Any())
                {
                    Console.WriteLine("[2.Step ]Files Found. Starting Processing...");
                    if (ConfigReader.MultiThread == 1)
                    {
                        Console.WriteLine("============================ MultiThread Service On (Safe Mode) ==============");

                      
                        int maxParallelFiles = ConfigReader.MaxThCount;
                        var semaphore = new SemaphoreSlim(maxParallelFiles);

                        var tasks = files.Select(async f =>
                        {
                           
                            await semaphore.WaitAsync();

                            try
                            {
                                Console.WriteLine($"[3.Step ] START {f.Name} Processing");

                               
                                await ProcessingFiles.ProcessFile(f.FullName);

                                Console.WriteLine($"[4.Step ] END {f.Name} Processing");
                            }
                            catch (Exception ex)
                            {
                                
                                Console.WriteLine($"[ERROR] File {f.Name} failed: {ex.Message}");
                            }
                            finally
                            {
                               
                                semaphore.Release();
                            }
                        });

                       
                        await Task.WhenAll(tasks);

                        Console.WriteLine("All 1000 files processed safely in multi-threaded mode.");
                    }
                    else
                    {
                        Console.WriteLine("========================Single Thread Service On");

                        foreach (var file in files)
                        {
                            Console.WriteLine($"=============START {file.Name} Proseesing=============");
                            await ProcessingFiles.ProcessFile(file.FullName);
                            Console.WriteLine($"============= END {file.Name} Proseesing==============");
                        }
                        Console.WriteLine("All files List processed in single-threaded mode.");
                    }
                }
                else
                {
                    Console.WriteLine($"No Files Found.Waiting...{ConfigReader.SleepTime} MiliSecound ");
                    await Task.Delay(ConfigReader.SleepTime);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Loop Error: {ex.Message}");
            }
            await Task.Delay(ConfigReader.SleepTime);
        }
      

    }


   
   
}