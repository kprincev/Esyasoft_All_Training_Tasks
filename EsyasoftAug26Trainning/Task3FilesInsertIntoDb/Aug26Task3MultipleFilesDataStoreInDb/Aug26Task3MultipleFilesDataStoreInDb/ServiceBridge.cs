
using System.Data;
using Aug26Task3MultipleFilesDataStoreInDb.Config;
using Aug26Task3MultipleFilesDataStoreInDb.DbService;
using Aug26Task3MultipleFilesDataStoreInDb.Service;

namespace Aug26Task3MultipleFilesDataStoreInDb.ServiceBridge
{
    public class ServiceBridge
    {
        public static void ProcessFiles(string folder,string pattern,Func<string, DataTable> converter)
        {
            foreach (string file in Directory.EnumerateFiles(folder, pattern))
            {
                try
                {
                    Console.WriteLine(
                        $"\nProcessing: {Path.GetFileName(file)}"
                    );

                    // STEP 1
                    Console.WriteLine("1. Converting file to DataTable...");

                    DataTable dt = converter(file);

                    Console.WriteLine($"DataTable created. Rows = {dt.Rows.Count}, Columns = {dt.Columns.Count}");


                    // STEP 2
                    Console.WriteLine("2. Converting DataTable to JSON...");

                    string json =Services.DataTableToJson(dt);

                    Console.WriteLine("JSON created:");
                    Console.WriteLine(json);


                    // STEP 3
                    Console.WriteLine("3. Inserting into database...");

                    DataBaseService.InsertIntoDatabase(json);

                    Console.WriteLine(  "Database insertion successful.");


                    // STEP 4
                    Console.WriteLine("4. Moving file to Processed...");

                    MoveFile(file,  ConfigReader.ProcessedFolder);

                    Console.WriteLine("File moved to Processed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n========== ERROR ==========");
                    Console.WriteLine($"File: {Path.GetFileName(file)}");
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Type: {ex.GetType().Name}");
                    Console.WriteLine("============================\n");

                    MoveFile(file,  ConfigReader.ErrorFolder);
                }
            }
        }

        private static void MoveFile(string source, string destinationFolder)
        {
            string destination =  Path.Combine( destinationFolder,Path.GetFileName(source));

            if (File.Exists(destination))
            {
                string name =Path.GetFileNameWithoutExtension(source);

                string ext = Path.GetExtension(source);

                destination =  Path.Combine(destinationFolder,$"{name}_{DateTime.Now:yyyyMMddHHmmss}{ext}");
            }

            File.Move(source, destination);
        }
    }
}