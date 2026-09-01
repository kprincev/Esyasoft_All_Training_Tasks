using RabbitMQ.Client;

namespace RabbitFilePipeline.Common
{
    public static class RabbitService
    {
        public static IConnection CreateConnection(string host)
        {
            var factory = new ConnectionFactory
            {
                HostName = host
            };
            return factory.CreateConnection();
        }

        public static void DeclareQueues(IModel channel)
        {
            channel.QueueDeclare("json_queue", true, false, false);
            channel.QueueDeclare("csv_queue", true, false, false);
            channel.QueueDeclare("xml_queue", true, false, false);
        }
    }
}
