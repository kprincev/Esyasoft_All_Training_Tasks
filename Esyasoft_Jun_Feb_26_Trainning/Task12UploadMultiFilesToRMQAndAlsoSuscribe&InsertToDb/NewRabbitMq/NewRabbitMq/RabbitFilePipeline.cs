using Newtonsoft.Json;
using RabbitFilePipeline.Common;
using RabbitMQ.Client;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace RabbitFilePipeline.Publisher
{
    public static class RetryPublisherService
    {
        public static void Run()
        {
            string host = ConfigurationManager.AppSettings["RabbitHost"];
            string errorFolder = ConfigurationManager.AppSettings["ErrorFolder"];

            if (!Directory.Exists(errorFolder))
                return;

            using (var connection = RabbitService.CreateConnection(host))
            using (var channel = connection.CreateModel())
            {
                RabbitService.DeclareQueues(channel);

                foreach (var file in Directory.GetFiles(errorFolder))
                {
                    try
                    {
                        if (!CanRetry(file))
                        {
                            Console.WriteLine($"Retry limit exceeded: {file}");
                            continue;
                        }

                        string newFile = GetNextRetryFileName(file);

                        // 🔥 FIX (file exist check)
                        if (File.Exists(newFile))
                        {
                            File.Delete(newFile);
                        }

                        File.Move(file, newFile);

                        string ext = Path.GetExtension(newFile).ToLower();
                        string queue = GetQueue(ext);

                        if (queue == null)
                            continue;

                        var msg = new FileMessage
                        {
                            FileName = Path.GetFileName(newFile),
                            FileType = ext,
                            FileContent = File.ReadAllText(newFile)
                        };

                        var body = Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(msg));

                        channel.BasicPublish("", queue, null, body);

                        Console.WriteLine($"Retry published: {msg.FileName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Retry error: " + ex.Message);
                    }
                }
            }
        }

        // ===============================
        // GET QUEUE NAME
        // ===============================
        static string GetQueue(string ext)
        {
            if (ext == ".json") return "json_queue";
            if (ext == ".csv") return "csv_queue";
            if (ext == ".xml") return "xml_queue";

            return null;
        }

        // ===============================
        // RETRY LIMIT
        // ===============================
        static bool CanRetry(string fileName)
        {
            return GetRetryCount(fileName) < 3;
        }

        // ===============================
        // NEXT RETRY NAME
        // ===============================
        static string GetNextRetryFileName(string filePath)
        {
            int retry = GetRetryCount(filePath) + 1;

            string dir = Path.GetDirectoryName(filePath);
            string ext = Path.GetExtension(filePath);

            string baseName =
                Path.GetFileNameWithoutExtension(filePath)
                .Split('.')[0];

            return Path.Combine(
                dir,
                $"{baseName}.retry{retry}{ext}"
            );
        }

        // ===============================
        // GET CURRENT RETRY COUNT
        // ===============================
        static int GetRetryCount(string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var parts = name.Split('.');

            foreach (var p in parts)
            {
                if (p.StartsWith("retry"))
                {
                    if (int.TryParse(p.Replace("retry", ""), out int n))
                        return n;
                }
            }

            return 0;
        }
    }
}