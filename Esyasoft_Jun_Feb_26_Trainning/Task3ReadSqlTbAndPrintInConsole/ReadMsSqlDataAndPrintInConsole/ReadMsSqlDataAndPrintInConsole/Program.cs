using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace ReadMsSqlDataAndPrintInConsole
{
    class DatabaseConfig
    {
        public string connectionString;
        public DatabaseConfig()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;

        }
    }
    public class Student
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int age { get; set; }

    }
    class Mathonds
    {
      
        public List<Student> ReadMsSqlData(string query)
        {

            DatabaseConfig dbConfig = new DatabaseConfig();
            List<Student> list = new List<Student>();
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConfig.connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    list.Add(new Student
                                    {
                                        Id = (int)reader["Stu_id"],
                                        name = reader["Stu_Name"].ToString(),
                                        age = (int)reader["Age"]
                                    });
                                }
                            }
                        }
                    }

                    conn.Close();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);

            }

            return list;

        }


        internal class Program
        {
            static void Main(string[] args)
            {
                Mathonds mathonds = new Mathonds();
                string query = "SELECT Stu_id, Stu_Name, Age FROM Student";
                var students = mathonds.ReadMsSqlData(query);
                foreach (var student in students)
                {
                    Console.WriteLine($"ID: {student.Id}, Name: {student.name}, Age: {student.age}");
                }
                Console.ReadLine();
            }
        }
    }
}
