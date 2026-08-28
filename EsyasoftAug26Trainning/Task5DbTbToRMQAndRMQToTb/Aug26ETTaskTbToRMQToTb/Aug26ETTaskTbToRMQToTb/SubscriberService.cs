using Aug26ETTaskTbToRMQToTb;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitFilePipeline
{
    public static class SubscriberService
    {
        public static void StartListening()
        {
            try
            {
                // 1. Connection Factory setup
                var factory = new ConnectionFactory
                {
                    HostName = ConfigReader.Host,
                    Port = ConfigReader.Port,
                    UserName = ConfigReader.UserName,
                    Password = ConfigReader.Password
                };

                Log.Information("RabbitMQ Consumer Connection ..");

                // 2. Connection aur Channel create karein (Synchronously test karne ke liye)
                Log.Information("Create tcp connection to rmq server");
                var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                Log.Information("Listening connection: TCP Connection Successfully");
                Log.Information("Create Channel Inside Tcp conncetion ");
                var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
                Log.Information(" Channel Created");


                // 3. Queue declare karein (Ye zaroori hai agar Consumer pehle run ho jaye)
                Log.Information("Declare Queue");
                channel.QueueDeclareAsync(
                    queue: ConfigReader.queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                ).GetAwaiter().GetResult();
                Log.Information($"Queue Declare Successfully =>name is ={ConfigReader.queueName}");

                Log.Information($" [*] Waiting for messages in '{ConfigReader.queueName}'. To exit press CTRL+C");

                var consumer = new AsyncEventingBasicConsumer(channel);

            
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    Log.Information("Received Msg From Rmq");
                    byte[] body = ea.Body.ToArray();

                    string jsonString = Encoding.UTF8.GetString(body);

                    try
                    {
                        Log.Information("Data Insert In To Db");
                        await DatabaseService.InsertDataForConsumer(jsonString);

                        Log.Information(" [SUCCESS] Data Success Fully inserted ");
                        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception dbEx)
                    {
                       
                        Log.Error($"\n [DB ERROR]: Database insert fail ");
                        Log.Error($" Details: {dbEx.Message}");

                    
                        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                
                channel.BasicConsumeAsync(
                    queue: ConfigReader.queueName,
                    autoAck: false,
                    consumer: consumer
                ).GetAwaiter().GetResult();

              
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Error($" [ERROR]: {ex.Message}");
            }
        }
    }
}