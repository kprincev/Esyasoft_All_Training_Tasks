using Newtonsoft.Json;
using RabbitFilePipeline.Common;
using RabbitMQ.Client;
using System.Configuration;
using System.IO;
using System.Text;

namespace RabbitFilePipeline.Publisher
{
    public static class PublisherService
    {
        public static void Run()
        {
            string host = ConfigurationManager.AppSettings["RabbitHost"];
            string randomFolder = ConfigurationManager.AppSettings["RandomFolder"];

            using (var connection = RabbitService.CreateConnection(host))
            {


                using (var channel = connection.CreateModel())
                {



                    RabbitService.DeclareQueues(channel);

                    foreach (var file in Directory.GetFiles(randomFolder))
                    {
                        string ext = Path.GetExtension(file).ToLower();
                        string queue = null;
                        switch (ext)
                        {
                            case ".json":
                                queue = "json_queue";
                                break;
                            case ".csv":
                                queue = "csv_queue";
                                break;
                            case ".xml":
                                queue = "xml_queue";
                                break;
                            default:
                                queue = null;
                                break;
                        }
                        if (queue == null) continue;

                        var msg = new FileMessage
                        {
                            FileName = Path.GetFileName(file),
                            FileType = ext,
                            FileContent = File.ReadAllText(file)
                        };

                        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                        channel.BasicPublish("", queue, null, body);
                    }
                    // Replace the switch expression with a standard switch statement for C# 7.3 compatibility
                   
                }
            }
        }
    }
}
