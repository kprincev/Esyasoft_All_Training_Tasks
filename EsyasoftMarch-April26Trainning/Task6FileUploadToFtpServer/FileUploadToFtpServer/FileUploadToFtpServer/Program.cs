using FileUploadToFtpServer;
using FluentFTP;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Diagnostics;

class Program
{

    static AsyncFtpClient ftpClient;
    static ConcurrentBag<AsyncFtpClient> ftpPool = new ConcurrentBag<AsyncFtpClient>();
    static int maxRetry = 3;
    static int TotalFiles = 0;
    static int ProcessedFiles = 0;
    public static async Task Main(string[] args)
    {
        string projectPath = Directory.GetParent(AppContext.BaseDirectory)
                                   .Parent.Parent.Parent.FullName;

        ConfigReader.config = new ConfigurationBuilder()
            .SetBasePath(projectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Directory.CreateDirectory(ConfigReader.ProcessedFolder);
  

        int poolSize = ConfigReader.MaxThCount;

        for (int i = 0; i < poolSize; i++)
        {
            var client = new AsyncFtpClient(
                ConfigReader.Host,
                ConfigReader.FtpUserName,
                ConfigReader.FtpPassword,
                ConfigReader.FtpPort
            );

            client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
            client.Config.ConnectTimeout = 30000;
            client.Config.ReadTimeout = 30000;

            await client.AutoConnect();

            ftpPool.Add(client);
        }

        Console.WriteLine($" FTP Pool Ready: {poolSize} connections");
    

        while (true)
        {
            Console.WriteLine("Checking files In Pending Folder ...");

            string[] allowedExtensions = ConfigReader.AllowedExtensions .Split(',')  .Select(x => x.Trim().ToLower()) .ToArray();

            Dictionary<string, Queue<string>> filesByFolder = Directory
                                                    .EnumerateFiles(ConfigReader.PendingFolder, "*.*", SearchOption.AllDirectories)
                                                    .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                                                    .GroupBy(f => Path.GetDirectoryName(f))
                                                    .ToDictionary(g => g.Key, g => new Queue<string>(g));
            TotalFiles = filesByFolder.Sum(f => f.Value.Count);

            if (!filesByFolder.Any(f => f.Value.Count > 0))
            {
                Console.WriteLine($"No files found. Sleeping...{ConfigReader.SleepTime} MiliSecound");
                await Task.Delay(ConfigReader.SleepTime);
                continue;
            }

            int batchSize = ConfigReader.BatchSize;
            int PerFolderBatch = ConfigReader.PerFolderBatch;
            int maxParallel = ConfigReader.MaxThCount;


            SemaphoreSlim semaphore = new SemaphoreSlim(maxParallel);

            while (filesByFolder.Any(f => f.Value.Count > 0))
            {
                List<string> batch = GetBalancedBatch(filesByFolder, batchSize, PerFolderBatch);

                if (batch.Count > 0)
                {
                    await ProcessBatchParallel(batch, semaphore);
                }
            }
        }
    }

    static async Task ProcessBatchParallel(List<string> batch, SemaphoreSlim semaphore)
    {
        IEnumerable<Task> tasks = batch.Select(async filePath =>
        {
            await semaphore.WaitAsync();
            try
            {
                await ProcessSingleFile(filePath);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    static async Task ProcessSingleFile(string filePath)
    {
        try
        {
            var file = new FileInfo(filePath);

            string relativePath = file.FullName
                .Replace(ConfigReader.PendingFolder, "")
                .TrimStart('\\')
                .Replace("\\", "/");

            string ftpPath = "/" + relativePath;

            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine($"[START] {file.Name}");

            await UploadFileFast(file.FullName, ftpPath);

            stopwatch.Stop();

            MoveToProcessed(file);

            Console.WriteLine($"[DONE] {file.Name} | Time: {stopwatch.Elapsed.TotalSeconds:F2} sec");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {filePath} -> {ex.Message}");
        }
    }

    

    public static void MoveToProcessed(FileInfo file)
    {
        string processedPath = file.FullName.Replace(
            ConfigReader.PendingFolder,
            ConfigReader.ProcessedFolder
        );

        Directory.CreateDirectory(Path.GetDirectoryName(processedPath));

        for (int i = 0; i < 3; i++)
        {
            try
            {
                File.Move(file.FullName, processedPath, true);
                break;
            }
            catch
            {
                Thread.Sleep(100);
            }
        }
    }

    static List<string> GetBalancedBatch(Dictionary<string, Queue<string>> filesByFolder, int batchSize,int PerFolderBatch)
    {
        List<string> batch = new List<string>();

        foreach (string folder in filesByFolder.Keys.ToList())
        {
            int count = 0;

            while (filesByFolder[folder].Count > 0 && count < PerFolderBatch && batch.Count < batchSize)
            {
                batch.Add(filesByFolder[folder].Dequeue());
                count++;
            }

            if (batch.Count >= batchSize)
                break;
        }


        return batch;
    }
    static async Task UploadFileFast(string localFilePath, string remoteFilePath)
    {
        int attempt = 0;

        while (attempt < maxRetry)
        {
            AsyncFtpClient client;

            while (!ftpPool.TryTake(out client))
            {
                await Task.Delay(10);
            }

            try
            {
                var status = await client.UploadFile(
                    localFilePath,
                    remoteFilePath,
                    FtpRemoteExists.Resume, 
                    createRemoteDir: true,
                    verifyOptions: FtpVerify.None
                );

                Console.WriteLine($" Uploaded: {Path.GetFileName(localFilePath)} - {status}");

                return;
            }
            catch (Exception ex)
            {
                attempt++;
                Console.WriteLine($" Retry {attempt}: {Path.GetFileName(localFilePath)} -> {ex.Message}");

                if (!client.IsConnected)
                {
                    try
                    {
                        await client.AutoConnect();
                    }
                    catch { }
                }

                if (attempt >= maxRetry)
                {
                    Console.WriteLine($"Failed: {Path.GetFileName(localFilePath)}");
                    throw;
                }

                await Task.Delay(500); 
            }
            finally
            {
                ftpPool.Add(client);
                Interlocked.Increment(ref ProcessedFiles);

                Console.WriteLine(
                    $"Progress: {ProcessedFiles}/{TotalFiles} | Remaining: {TotalFiles - ProcessedFiles}"
                );
            }
        }
    }
}