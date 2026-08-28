using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aug26ETTaskTbToRMQToTb
{
    public  class PublisherAPICallService
    {
        public async Task PublisherApiCall(string json)
        {
            //Combine json data and queue name in one object so send both 
            var AllPayload = new { JsonData = json, QueueName = ConfigReader.queueName };
            Log.Information($"sent json to queue name = {ConfigReader.queueName}");
            // Serialize combine object to json string to send api 
            string body = System.Text.Json.JsonSerializer.Serialize(AllPayload);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(ConfigReader.Url, content);

                    string responseString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Your Json Is Succseefully Publish");
                        Console.WriteLine("So Update Counter ... ");
                        DatabaseService.UpDateCounter();
                        Console.WriteLine("Counter Update Succeefully");
                    }
                    else
                    {
                        Console.WriteLine(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
