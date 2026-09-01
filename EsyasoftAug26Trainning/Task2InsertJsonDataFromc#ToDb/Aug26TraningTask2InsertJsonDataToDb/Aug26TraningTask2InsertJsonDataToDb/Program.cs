using System;
using System.Data;
using System.IO;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        string json = File.ReadAllText(configuration["JsonFilePath"]);
     

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            using SqlConnection con = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            con.Open();

            using SqlTransaction transaction = con.BeginTransaction();

            try
            { 
                int companyId = root.GetProperty("companyId").GetInt32();
                string companyName = root.GetProperty("companyName").GetString()!;
                JsonElement location = root.GetProperty("location");
                string country = location.GetProperty("country").GetString()!;
                string state = location.GetProperty("state").GetString()!;
                string city = location.GetProperty("city").GetString()!;

                using (SqlCommand cmd = new SqlCommand("sp_InsertCompany", con, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@COMPID", SqlDbType.Int).Value = companyId;
                    cmd.Parameters.Add("@COMPNAME", SqlDbType.VarChar, 50).Value = companyName;
                    cmd.Parameters.Add("@COUNTRY", SqlDbType.VarChar, 30).Value = country;
                    cmd.Parameters.Add("@STATE", SqlDbType.VarChar, 30).Value = state;
                    cmd.Parameters.Add("@CITY", SqlDbType.VarChar, 40).Value = city;
                    cmd.ExecuteNonQuery();
                }

    

                foreach (JsonElement department in root.GetProperty("departments").EnumerateArray())
                {
                    int departmentId = department.GetProperty("departmentId").GetInt32();
                    string departmentName = department.GetProperty("departmentName").GetString()!;

                    using (SqlCommand cmd = new SqlCommand("sp_InsertDepartment", con, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@DEPTID", SqlDbType.Int).Value = departmentId;
                        cmd.Parameters.Add("@COMPID", SqlDbType.Int).Value = companyId;
                        cmd.Parameters.Add("@DEPTNAME", SqlDbType.VarChar, 30).Value = departmentName;
                        cmd.ExecuteNonQuery();
                    }


                    foreach (JsonElement employee in department.GetProperty("employees").EnumerateArray())
                    {
                        int employeeId = employee.GetProperty("employeeId").GetInt32();
                        string employeeName = employee.GetProperty("name").GetString()!;
                        string email = employee.GetProperty("email").GetString()!;

                        using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@EMPID", SqlDbType.Int).Value = employeeId;
                            cmd.Parameters.Add("@EMPNAME", SqlDbType.VarChar, 30).Value = employeeName;
                            cmd.Parameters.Add("@DEPTID", SqlDbType.Int).Value = departmentId;
                            cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 50).Value = email;
                            cmd.ExecuteNonQuery();
                        }

          

                        JsonElement salary = employee.GetProperty("salary");
                        int basic = salary.GetProperty("basic").GetInt32();
                        int bonus = salary.GetProperty("bonus").GetInt32();
                        string currency = salary.GetProperty("currency").GetString()!;

                        using (SqlCommand cmd = new SqlCommand("sp_InsertSalary", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@EMPID", SqlDbType.Int).Value = employeeId;
                            cmd.Parameters.Add("@BASIC", SqlDbType.Int).Value = basic;
                            cmd.Parameters.Add("@BONUS", SqlDbType.Int).Value = bonus;
                            cmd.Parameters.Add("@CURRENCY", SqlDbType.VarChar, 10).Value = currency;
                            cmd.ExecuteNonQuery();
                        }

          

                        foreach (JsonElement project in employee.GetProperty("projects").EnumerateArray())
                        {
                            int projectId = project.GetProperty("projectId").GetInt32();
                            string projectName = project.GetProperty("projectName").GetString()!;
                            string projectStatus = project.GetProperty("status").GetString()!;

                            using (SqlCommand cmd = new SqlCommand("sp_InsertProject", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@PROJECTID", SqlDbType.Int).Value = projectId;
                                cmd.Parameters.Add("@EMPID", SqlDbType.Int).Value = employeeId;
                                cmd.Parameters.Add("@PROJECTNAME", SqlDbType.VarChar, 50).Value = projectName;
                                cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 20).Value = projectStatus;
                                cmd.ExecuteNonQuery();
                            }


                            foreach (JsonElement task in project.GetProperty("tasks").EnumerateArray())
                            {
                                int taskId = task.GetProperty("taskId").GetInt32();
                                string taskName = task.GetProperty("taskName").GetString()!;
                                string taskStatus = task.GetProperty("status").GetString()!;
                                int hours = task.GetProperty("hours").GetInt32();

                                using (SqlCommand cmd = new SqlCommand("sp_InsertTask", con, transaction))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("@TASKID", SqlDbType.Int).Value = taskId;
                                    cmd.Parameters.Add("@PROJECTID", SqlDbType.Int).Value = projectId;
                                    cmd.Parameters.Add("@TASKNAME", SqlDbType.VarChar, 50).Value = taskName;
                                    cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 20).Value = taskStatus;
                                    cmd.Parameters.Add("@HOURS", SqlDbType.Int).Value = hours;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                transaction.Commit();
                Console.WriteLine("All data inserted successfully.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Transaction rolled back.");
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine("Invalid JSON: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}