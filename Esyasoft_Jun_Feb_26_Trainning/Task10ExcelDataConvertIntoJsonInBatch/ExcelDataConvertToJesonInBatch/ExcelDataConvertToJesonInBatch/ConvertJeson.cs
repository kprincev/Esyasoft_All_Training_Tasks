using ClosedXML.Excel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;

namespace ExcelDataConvertToJesonInBatch
{
    public class ConvertJeson
    {
        public static void ProcessExcelWithResume(string filePath, int batchSize)
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                var allData = rows.Select(row => new {
                    Meter_id = row.Cell(1).GetValue<string>(),
                    Consumer_No = row.Cell(2).GetValue<string>(),
                    Meter_Phase = row.Cell(3).GetValue<string>(),
                    Reading_Date = row.Cell(4).GetValue<DateTime>().ToString("yyyy-MM-dd"),
                    V = row.Cell(5).GetValue<double>(),
                    C = row.Cell(6).GetValue<double>()
                }).ToList();

                int totalRows = allData.Count;

                int lastProcessedIndex = int.Parse(ConfigurationManager.AppSettings["LastProcessedIndex"] ?? "-1");

                if (lastProcessedIndex >= totalRows - 1)
                {
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("Data is already fully processed!");
                    Console.WriteLine("To restart, set LastProcessedIndex to -1 in App.config.");
                    Console.WriteLine("------------------------------------------------------");
                    return;
                }

               
                int startIndex = lastProcessedIndex + 1;

                var currentBatch = allData.Skip(startIndex).Take(batchSize).ToList();

                string fileName = $"Batch_Starting_At_{startIndex}.json";
                string fullPath = Path.Combine(ConfigReader.outputPath, fileName);

                File.WriteAllText(fullPath, JsonSerializer.Serialize(currentBatch, new JsonSerializerOptions { WriteIndented = true }));

                // 6. Naya Index aur Remaining calculate karo
                int newLastIndex = startIndex + currentBatch.Count - 1;
                int remaining = totalRows - (newLastIndex + 1);

                UpdateConfig.UpdateAppConfig(newLastIndex, "LastProcessedIndex");
                UpdateConfig.UpdateAppConfig(remaining, "RemainingRows");

                Console.WriteLine($"Batch Complete: {currentBatch.Count} records processed.");
                Console.WriteLine($"Current Index position: {newLastIndex}");
                Console.WriteLine($"Total Pending: {remaining} records.");
            }
        }
    }
}