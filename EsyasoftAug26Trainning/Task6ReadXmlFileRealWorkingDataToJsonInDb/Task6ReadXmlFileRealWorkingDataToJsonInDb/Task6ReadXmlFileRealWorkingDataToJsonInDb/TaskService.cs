using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Task6ReadXmlFileRealWorkingDataToJsonInDb
{
    public class TaskService
    {
        public void ProcessFiles(string PayloadPath)
        {
            Console.WriteLine($"\n--- Files Scan : {PayloadPath} ---");
            Console.WriteLine($"Max Parallel Threads Allowed: {ConfigReader.MaxThread}");

            IEnumerable<List<string>> fileBatches;

            if (ConfigReader.ServiceMode == 0)
            {
                fileBatches = Directory.EnumerateFiles(PayloadPath, "*.xml", SearchOption.AllDirectories)
                    .Select(file => new
                    {
                        FilePath = file,
                        PayloadName = Directory.GetParent(file)?.Name ?? "UNKNOWN"
                    })
                    .GroupBy(x => x.PayloadName)
                    .SelectMany(group => group.Select(x => x.FilePath).Chunk(ConfigReader.FileBatchSize).Select(chunk => chunk.ToList()));
            }
            else
            {
                fileBatches = Directory.EnumerateFiles(PayloadPath, "*.xml", SearchOption.AllDirectories)
                    .Chunk(ConfigReader.FileBatchSize)
                    .Select(chunk => chunk.ToList());
            }

            int batchNumber = 1;
            int totalProcessedFiles = 0;

            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = ConfigReader.MaxThread
            };

            // Total Execution Stopwatch
            Stopwatch totalTimer = Stopwatch.StartNew();

            // Continuous Batch Loop
            foreach (var currentBatchFiles in fileBatches)
            {
                if (currentBatchFiles.Count == 0) break;

                Console.WriteLine($"\nBatch #{batchNumber} start To Process ({currentBatchFiles.Count} files)...");

                // Batch Execution Stopwatch
                Stopwatch batchTimer = Stopwatch.StartNew();

                // Parallel Processing Loop
                Parallel.ForEach(currentBatchFiles, parallelOptions, xmlFilePath =>
                {
                    string fileName = Path.GetFileName(xmlFilePath);
                    DirectoryInfo parentDirInfo = Directory.GetParent(xmlFilePath);
                    string PaylodFolderName = parentDirInfo?.Name ?? "UNKNOWN";

                    Console.WriteLine($"[Thread-{Task.CurrentId}] Reading File: {fileName} | Payload: {PaylodFolderName}");

                    try
                    {
                        string jsonOutput = XmlFileReadingService.IpFileReadMethod(xmlFilePath);

                        if (ConfigReader.JsonFileCreateMode.ToLower() == "y")
                        {
                            string jsonFileName = Path.ChangeExtension(fileName, ".json");
                            DataBaseService ob = new DataBaseService();

                            try
                            {
                                ob.InsertJsonIPData(jsonOutput);

                                File.WriteAllText(HelperMethods.GetDynamicDestinationPath(ConfigReader.ProcessedJsonFolder, PaylodFolderName, jsonFileName), jsonOutput);
                                Console.WriteLine($"-> [Thread-{Task.CurrentId}] DB insert complete & JSON saved for {fileName}.");
                            }
                            catch
                            {
                                File.WriteAllText(HelperMethods.GetDynamicDestinationPath(ConfigReader.DbErrorJsonFolder, PaylodFolderName, jsonFileName), jsonOutput);
                                HelperMethods.MoveFile(xmlFilePath, HelperMethods.GetDynamicDestinationPath(ConfigReader.DBErrorFilesFolder, PaylodFolderName, fileName));
                                Console.WriteLine($"-> [Thread-{Task.CurrentId}] DB insert fail: {fileName} moved to DbErrorFolder.");
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                DataBaseService ob = new DataBaseService();
                                ob.InsertJsonIPData(jsonOutput);
                                Console.WriteLine($"-> [Thread-{Task.CurrentId}] DB insert complete for {fileName}.");
                            }
                            catch (Exception dbEx)
                            {
                                Console.WriteLine($"-> [Thread-{Task.CurrentId}] DB Error on {fileName}: {dbEx.Message}");
                                HelperMethods.MoveFile(xmlFilePath, HelperMethods.GetDynamicDestinationPath(ConfigReader.DBErrorFilesFolder, PaylodFolderName, fileName));
                                Console.WriteLine($"-> [Thread-{Task.CurrentId}] XML Moved To DbErrorFolder.");
                                return;
                            }
                        }

                        HelperMethods.MoveFile(xmlFilePath, HelperMethods.GetDynamicDestinationPath(ConfigReader.ProcessedFolder, PaylodFolderName, fileName));
                        Console.WriteLine($"-> [Thread-{Task.CurrentId}] Successfully processed & Moved to Processed: {fileName}");
                    }
                    catch (KeyNotFoundException ex)
                    {
                        Console.WriteLine($"-> [Thread-{Task.CurrentId}] Validation Error on {fileName}: {ex.Message}");
                        HelperMethods.MoveFile(xmlFilePath, HelperMethods.GetDynamicDestinationPath(ConfigReader.ReadErrorFilesFolder, PaylodFolderName, fileName));
                        Console.WriteLine($"-> [Thread-{Task.CurrentId}] Moved to ReadError: {fileName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"-> [Thread-{Task.CurrentId}] Read/Parsing Error on {fileName}: {ex.Message}");
                        HelperMethods.MoveFile(xmlFilePath, HelperMethods.GetDynamicDestinationPath(ConfigReader.ReadErrorFilesFolder, PaylodFolderName, fileName));
                        Console.WriteLine($"-> [Thread-{Task.CurrentId}] Moved to ReadError: {fileName}");
                    }
                });

                batchTimer.Stop();
                totalProcessedFiles += currentBatchFiles.Count;


                Console.WriteLine($" Batch #{batchNumber} Complete in: {batchTimer.Elapsed.TotalSeconds:F2} seconds ({batchTimer.ElapsedMilliseconds} ms)");
                batchNumber++;
            }

            totalTimer.Stop();

            Console.WriteLine("\n==================================================");
            Console.WriteLine($" === All Files Processed ===");
            Console.WriteLine($" Total Files Processed: {totalProcessedFiles}");
            Console.WriteLine($" Total Time Taken: {totalTimer.Elapsed.TotalSeconds:F2} seconds ({totalTimer.ElapsedMilliseconds} ms)");
            Console.WriteLine("==================================================\n");
        }
    }
}