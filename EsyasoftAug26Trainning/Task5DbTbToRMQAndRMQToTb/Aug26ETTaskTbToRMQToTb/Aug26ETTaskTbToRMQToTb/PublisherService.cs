using Aug26ETTaskTbToRMQToTb;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitFilePipeline
{
    public static class PublisherService
    {
        public static void PublishJson(string json)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory
                {
                    HostName = ConfigReader.Host,
                    Port = ConfigReader.Port,
                    UserName = ConfigReader.UserName,
                    Password = ConfigReader.Password
                };

                Console.WriteLine("Connection To RabbitMQ  ...");

          
                using IConnection connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                using IChannel channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

           

                Console.WriteLine("Queue Declare ...");
                channel.QueueDeclareAsync(
                    queue: ConfigReader.queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                ).GetAwaiter().GetResult();

                byte[] body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                Console.WriteLine("Publish massage .....");
                channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: ConfigReader.queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                ).GetAwaiter().GetResult();

                Console.WriteLine(" [SUCCESS] Message Sent to RabbitMQ!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [ERROR DETAIL]: {ex.ToString()}");
            }
        }
    }
}