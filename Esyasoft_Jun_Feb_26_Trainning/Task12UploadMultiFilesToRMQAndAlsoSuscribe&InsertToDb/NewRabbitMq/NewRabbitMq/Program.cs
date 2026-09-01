using RabbitFilePipeline.Publisher;
using RabbitFilePipeline.Subscriber;
using RabbitFilePipeline.Processor;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitFilePipeline
{
    class Program
    {
        static void Main()
        {
            string mode = ConfigurationManager.AppSettings["AppMode"];

            if (mode == "Publisher")
            {
                PublisherService.Run();
                Console.WriteLine("Publish successfully");
            }

            else if (mode == "Subscriber")
            {
                RunSubscribersAndBackgroundTasks();
            }

            else if (mode == "All")
            {
                PublisherService.Run();
                Console.WriteLine("Publish successfully");

                RunSubscribersAndBackgroundTasks();
            }

            Console.WriteLine("Application finished.");
        Console.ReadLine();

        }


        static void RunSubscribersAndBackgroundTasks()
        {
            int subscriberCount =
                int.Parse(ConfigurationManager.AppSettings["SubscriberCount"]);

            Task[] subscriberTasks = new Task[subscriberCount];

            // 🔹 Subscribers
            for (int i = 0; i < subscriberCount; i++)
            {
                int index = i + 1;

                subscriberTasks[i] = Task.Run(() =>
                {
                    Console.WriteLine($"Subscriber #{index} started");
                    SubscriberService.Run();
                    Console.WriteLine($"Subscriber #{index} finished");
                });
            }

            // 🔹 Processor (continuous)
            Task processorTask = Task.Run(() =>
            {
                while (true)
                {
                    PendingProcessor.Run();
                    Thread.Sleep(2000); // every 2 seconds
                }
            });

            // 🔹 Retry Publisher (continuous)
            Task retryTask = Task.Run(() =>
            {
                while (true)
                {
                    RetryPublisherService.Run();
                    Thread.Sleep(10000); // every 10 sec
                }
            });

            Task.WaitAll(subscriberTasks);

            Console.WriteLine("Subscribers finished.");
        }
    }
}
