using Aug26ETTaskTbToRMQToTb;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RabbitFilePipeline;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Serilog;


class Program
{
    public static void Main(String[] args)
    {
        Console.WriteLine("Service is start...");
        HostApplicationBuilder builder=Host.CreateApplicationBuilder(args);
         ConfigReader.config = builder.Configuration;

        Console.WriteLine("[1].First Create Logger To globley ...");
        Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File(ConfigReader.logPath,rollingInterval:RollingInterval.Day).CreateLogger();
        Log.Information("[1] Application Start With loging Steps...");
        Log.Information("[2] Check Application Mode Y (Publish Msg Thourgh Api) Or N (Suscriber Msg) ");
           if(ConfigReader.ServiceMode.ToLower()=="y")
            {
                  
                 Log.Information("[3] Application Mode Y Publish Msg Thourgh APi Start");
                try
                {
                Log.Information("[4] Get Data From DB ");
                     string json = DatabaseService.GetDataForPuplisher();

                        if(json=="")
                        {
                             Console.WriteLine("No Data Found In Table To Publish...");
                              return;
                        }
                // create object to call method of publisherapicallservice class

                       PublisherAPICallService ob=new PublisherAPICallService();
                Log.Information("Call To Api To sent Json Call Methond PublisherApiCall");
                ob.PublisherApiCall(json).Wait();
                Log.Information("Json is Sent Successfully");


                Log.Information("Service End");
                }
                catch (Exception ex)
                {
                Log.Error(ex.Message);
                }
            }
            else
            {

            Log.Information("[3] Application Mode N Suscribe Msg From Rmq start");
                  SubscriberService.StartListening();
            Log.Information("Service End ");
            }

    }
}
