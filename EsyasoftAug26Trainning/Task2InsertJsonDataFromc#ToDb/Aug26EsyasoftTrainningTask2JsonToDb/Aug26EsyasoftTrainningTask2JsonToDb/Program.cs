using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
class Program
{
    public static void Main(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        string json = "";
         json = File.ReadAllText(configuration["JsonFilePath"]);
        try
        {


            using (SqlConnection Conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                Conn.Open();
                using (SqlCommand cmd = new SqlCommand(configuration["SP_InsertData"], Conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JSON", json);
                    cmd.ExecuteNonQuery();

                }
                Console.WriteLine("Data inserted Sussfully ...");
                Conn.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
