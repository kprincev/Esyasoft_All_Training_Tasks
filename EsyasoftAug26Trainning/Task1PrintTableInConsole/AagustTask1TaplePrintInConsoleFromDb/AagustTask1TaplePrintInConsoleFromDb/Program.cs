using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;


class Program
{
    public static void Main(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
 
     
        using(SqlConnection Conn=new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            Conn.Open();
            using (SqlCommand cmd=new SqlCommand(configuration["GetDataSP"],Conn))
            {
                cmd.CommandType=System.Data.CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
               
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i),-20}");
                    
                }
                Console.WriteLine();
                while (reader.Read())
                {
                    for(int i=0;i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader[i],-20}");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
             
            }
            Conn.Close();

        }



    }
}





