using MeterBatchProcessor.Config;
using System;
using System.IO;

namespace MeterBatchProcessor.Services
{
    public static class BatchService
    {
        public static void Run()
        {
            Directory.CreateDirectory(
                Path.Combine(ConfigReader.BasePath, "process"));

            Directory.CreateDirectory(
                Path.Combine(ConfigReader.BasePath, "error"));

            while (FileService.PendingCount() > 0)
            {
                Console.WriteLine("\n=======================================");
                Console.WriteLine(" NEW BATCH STARTED");
                Console.WriteLine("=======================================");

                FileService.ProcessBatch(ConfigReader.BatchSize);
                DatabaseService.InsertProcessedFiles();

                Console.WriteLine("\n Batch completed, moving to next...\n");
            }


            Console.WriteLine(" All files processed");
        }
    }
}
