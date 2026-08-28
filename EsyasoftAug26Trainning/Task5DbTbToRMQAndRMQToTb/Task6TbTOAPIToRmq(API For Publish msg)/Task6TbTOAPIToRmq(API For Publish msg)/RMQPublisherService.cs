using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.HttpResults;
using RabbitMQ.Client;
using System.Text;
using Serilog;
namespace Task6TbTOAPIToRmq_API_For_Publish_msg_
{
    public class RMQPublisherService
    {
        public void PublishMeassage(string json,string queueName,IConfiguration ConfigReader)
        {
            
                ConnectionFactory factory = new ConnectionFactory
                {
                    HostName = ConfigReader["HostConfig:Host"],
                    Port = int.Parse(ConfigReader["HostConfig:Port"]),//Parse For Because Port is int and appsettings give as a string 
                    UserName = ConfigReader["HostConfig:UserName"],
                    Password = ConfigReader["HostConfig:Password"]  
                };

                Log.Information("Connection To RabbitMQ  ...");


                using IConnection connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                using IChannel channel = connection.CreateChannelAsync().GetAwaiter().GetResult();



                Console.WriteLine("Queue Declare ...");
                channel.QueueDeclareAsync(
                    queue: queueName,
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

                Log.Information("Publish massage .....");
                channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                ).GetAwaiter().GetResult();

            Log.Information(" [SUCCESS] Message Sent to RabbitMQ!");
            
            
        }
    }
}
