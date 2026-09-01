using chatgptBlpInsertdata;
using System;
using System.IO;
using System.Linq;

public class BatchProcessor
{
    public void Start()
    {
      
        Directory.CreateDirectory(AppConfig.Processed);
        Directory.CreateDirectory(AppConfig.ReadError);
        Directory.CreateDirectory(AppConfig.DbError);

        while (true)
        {
            var files = Directory.GetFiles(AppConfig.Pending, "*.txt")
                                 .Take(10)
                                 .ToList();

            if (files.Count == 0)
                break;

            foreach (var file in files)
            {
                ProcessSingleFile(file);
            }
        }
    }

    private void ProcessSingleFile(string file)
    {
        try
        {
            if (!FileValidator.IsValid(file))
            {
                FileMover.Move(file, AppConfig.ReadError);
                Console.WriteLine(Path.GetFileName(file)+" : file found read error So file Move ReadError Folder");
                return;
            }
            var table = FileParser.Parse(file);

            DatabaseService.BulkInsert(table);
            FileMover.MoveToProcessedWithDate(file, AppConfig.Processed);

            Console.WriteLine($"{Path.GetFileName(file)} : Successfully processed");
        }
        catch
        {
            FileMover.SafeMove(file, AppConfig.DbError);
            Console.WriteLine(Path.GetFileName(file) + " : file found insert error to file move to Db error folder ");
        }
    }
}
