using Newtonsoft.Json;
using RabbitFilePipeline.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;

namespace RabbitFilePipeline.Subscriber
{
    public static class SubscriberService
    {
        public static void Run()
        {
            int size =int.Parse( ConfigurationManager.AppSettings["size"]);
            Consume("json_queue", size);
            Consume("csv_queue", size);
            Consume("xml_queue", size);

            Console.WriteLine($"Subscriber finished ({size}JSON, {size}CSV, {size}XML).");
        }

        static void Consume(string queueName, int limit)
        {
            string host = ConfigurationManager.AppSettings["RabbitHost"];
            string pendingFolder = ConfigurationManager.AppSettings["PendingFolder"];

            Directory.CreateDirectory(pendingFolder);

            using (var connection = RabbitService.CreateConnection(host))
            using (var channel = connection.CreateModel())
            {
               
                channel.BasicQos(0, (ushort)limit, false);

                int count = 0;
                string consumerTag = null;

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, e) =>
                {
                    try
                    {
                        if (count >= limit)
                            return;

                        var msg = JsonConvert.DeserializeObject<FileMessage>(
                            Encoding.UTF8.GetString(e.Body.ToArray()));

                        // recreate file
                        File.WriteAllText(
                            Path.Combine(pendingFolder, msg.FileName),
                            msg.FileContent
                        );

                        channel.BasicAck(e.DeliveryTag, false);
                        count++;

                        Console.WriteLine($"{queueName} → file created: {msg.FileName}");

                        // stop consuming AFTER limit
                        if (count == limit)
                        {
                            channel.BasicCancel(consumerTag);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Subscriber error: " + ex.Message);
                        channel.BasicNack(e.DeliveryTag, false, true);
                    }
                };

                consumerTag = channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer
                );

                // wait until all messages processed
                while (count < limit)
                {
                    Thread.Sleep(200);
                }
            }
        }
    }
}
