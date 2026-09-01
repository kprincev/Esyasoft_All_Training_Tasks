using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DistributionConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {


                    int Lastid=0 ;
                    int fetchCount = 0;
                    Console.WriteLine("============================[Batch Start]=================================");
                    Console.WriteLine("[1.Step]=> Read Counter Table ....");
                    using (SqlConnection con = new SqlConnection(ConfigReader.conStr))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("GetMeterCounter", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@MeterType", ConfigReader.meterType);
                            cmd.Parameters.AddWithValue("@BLPYear", ConfigReader.year);

                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                Lastid = Convert.ToInt32(reader["Lastid"]);
                                fetchCount = Convert.ToInt32(reader["FetchCount"]);
                            }
                        }

                        con.Close();
                    }

                    var requestObj = new
                    {
                        start = Lastid,
                        count = fetchCount,
                        meter_type = ConfigReader.meterType,
                        blpyear = ConfigReader.year
                    };

                    string requestJson = JsonConvert.SerializeObject(requestObj);

                    string responseJson = "";
                    Console.WriteLine("[2.Step]=>Request for Fetching Data from API...");
                    using (HttpClient client = new HttpClient())
                    {

                        var byteArray = Encoding.ASCII.GetBytes(ConfigReader.UserPassword);

                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(
                                "Basic",
                                Convert.ToBase64String(byteArray));
                        StringContent content = new StringContent(
                                requestJson,
                                Encoding.UTF8,
                                "application/json");

                        HttpResponseMessage response = await client.PostAsync(ConfigReader.apiUrl, content);

                       
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            string errorMessage = await response.Content.ReadAsStringAsync();

                            Console.WriteLine(
                                $"API Request Failed with Status Code: {response.StatusCode} => {errorMessage}. Waiting {ConfigReader.Dtime}  MiliSecound...");
                            await Task.Delay(ConfigReader.Dtime);
                            continue;
                        }
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                                Console.WriteLine("[3.Step ]=>Response  received from API, processing...");
                                responseJson = await response.Content.ReadAsStringAsync();
                        }
                        
                        if (responseJson == "[]" || string.IsNullOrWhiteSpace(responseJson))
                        {
                            Console.WriteLine($"No Data Found. Waiting{ConfigReader.Dtime}  MiliSecound...");

                            await Task.Delay(ConfigReader.Dtime);

                            continue;
                        }

                    }
                    int max = DataBaseService.InsertData(responseJson);
                    Console.WriteLine($"[4.Step]=>Data Inserted Succefully from {Lastid} To {max} & Counter Update Succesfully");
                    Console.WriteLine("Data Sync Completed");
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}. Waiting  {ConfigReader.Dtime}  MiliSecound before retrying...");
                    await Task.Delay(ConfigReader.Dtime);
                }

            }
        }
        
        
    }
}