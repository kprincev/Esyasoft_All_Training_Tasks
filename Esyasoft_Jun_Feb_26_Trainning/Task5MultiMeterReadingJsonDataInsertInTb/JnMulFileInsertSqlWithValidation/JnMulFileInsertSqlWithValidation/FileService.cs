using MeterBatchProcessor.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MeterBatchProcessor.Services
{
    public static class FileService
    {
        public static void ProcessBatch(int batchSize)
        {
            var files = Directory.GetFiles(ConfigReader.PendingPath, "*.json")
                                 .Take(batchSize)
                                 .ToList();

            HashSet<string> batchKeys = new HashSet<string>();
            int index = 0;

            Console.WriteLine($"\n--- FILE VALIDATION (Batch Size: {files.Count}) ---");

            foreach (var file in files)
            {
                index++;
                string fileName = Path.GetFileName(file);
                Console.WriteLine($"\n[{index}/{files.Count}] {fileName}");

                try
                {
                    string json = File.ReadAllText(file);
                    var data = Newtonsoft.Json.JsonConvert
                                .DeserializeObject<Models.MeterReading>(json);

                    if (data == null || string.IsNullOrEmpty(data.MeterId))
                    {
                        Console.WriteLine(" ❌ INVALID JSON or MeterId missing");
                        MoveToError(file);
                        continue;
                    }

                    string key = data.MeterId + "|" +
                                 data.ReadingDate.ToString("yyyyMMdd");

                    if (batchKeys.Contains(key))
                    {
                        Console.WriteLine(" ❌ Duplicate in SAME BATCH");
                        MoveToError(file);
                        continue;
                    }

                    batchKeys.Add(key);

                    MoveToProcess(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" ❌ EXCEPTION: {ex.Message}");
                    MoveToError(file);
                }
            }
        }
    


        public static int PendingCount() =>
            Directory.GetFiles(ConfigReader.PendingPath, "*.json").Length;

        static void MoveToProcess(string file)
        {
            string dest = Path.Combine(ConfigReader.ProcessPath, Path.GetFileName(file));
            if (File.Exists(dest)) File.Delete(dest);
            File.Move(file, dest);
        }

        static void MoveToError(string file)
        {
            string dest = Path.Combine(ConfigReader.ErrorPath, Path.GetFileName(file));
            if (File.Exists(dest)) File.Delete(dest);
            File.Move(file, dest);
        }
    }
}
