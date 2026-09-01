using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApiDataInserter;
using WeatherApiDataInserter.ModelClass;

class Program
{
    static async Task Main()
    {
        Console.Write("Enter City Name:");
        string city = Console.ReadLine();

        string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={ConfigReader.apikeyy}&units=metric";

        using (HttpClient client = new HttpClient())
        {
            string json = await client.GetStringAsync(apiUrl);

            WeatherResponse response =
            JsonConvert.DeserializeObject<WeatherResponse>(json);

            DateTime weatherDate =
                DateTimeOffset.FromUnixTimeSeconds(response.dt).DateTime;

            DatabaseServicecs.InsertWeather(
                response.name,
                response.main.temp,
                weatherDate
            );
        }

        Console.WriteLine("Weather data inserted successfully ✅");
    }

    
}
