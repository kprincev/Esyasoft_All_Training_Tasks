using MeterBatchProcessor.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace MeterBatchProcessor.Services
{
    public static class DatabaseService
    {
        public static void InsertProcessedFiles()
        {
            var files = Directory
                .GetFiles(ConfigReader.ProcessPath, "*.json")
                .Where(f => !f.EndsWith(".done.json") &&
                            !f.EndsWith(".error.json"))
                .ToList();

            Console.WriteLine($"\n--- DATABASE INSERT ({files.Count} files) ---");

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                Console.WriteLine($"\n[DB] Processing: {fileName}");

                try
                {
                    // 🔹 Read JSON file
                    string json = File.ReadAllText(file);

                    using (SqlConnection conn =
                        new SqlConnection(ConfigReader.ConnectionString))
                    {
                        conn.Open();

                        using (SqlCommand cmd =
                            new SqlCommand("InsertMeterReadingg", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // 🔹 JSON parameter (NVARCHAR(MAX))
                            SqlParameter jsonParam =
                                new SqlParameter("@json", SqlDbType.NVarChar, -1);
                            jsonParam.Value = json;
                            cmd.Parameters.Add(jsonParam);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine($" {fileName} ✅ INSERT SUCCESS");
                    RenameStatus(file, "done");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($" {fileName} ❌ INSERT FAILED");
                    Console.WriteLine($"    SQL Reason: {ex.Message}");

                    RenameStatus(file, "error");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" {fileName} ❌ GENERAL ERROR");
                    Console.WriteLine($"    Reason: {ex.Message}");

                    RenameStatus(file, "error");
                }
            }
        }

        private static void RenameStatus(string file, string status)
        {
            string baseName =
                Path.GetFileNameWithoutExtension(file).Split('.')[0];

            string dest = Path.Combine(
                ConfigReader.ProcessPath,
                $"{baseName}.{status}.json");

            if (File.Exists(dest))
                File.Delete(dest);

            File.Move(file, dest);
        }
    }
}
