using InsertBplData;

using System.IO;
using System.Linq;

public class BatchProcessor
{
    public void Start()
    {
        Directory.CreateDirectory(ConfigReader.Processing);
        Directory.CreateDirectory(ConfigReader.Processed);
        Directory.CreateDirectory(ConfigReader.SyntaxError);
        Directory.CreateDirectory(ConfigReader.ReadError);
        Directory.CreateDirectory(ConfigReader.DbError);

        while (true)
        {
            var files = Directory.GetFiles(ConfigReader.Pending, "*.txt")
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
                FileMover.Move(file, ConfigReader.ReadError);
                return;
            }

            string processingFile = FileMover.Move(file, ConfigReader.Processing);
            string rawData = File.ReadAllText(processingFile);
            var records=SplitService.SplitServe(rawData);

            DatabaseService.Database(records);

            FileMover.Move(processingFile, ConfigReader.Processed);
        }
        catch
        {
            FileMover.SafeMove(file, ConfigReader.DbError);
        }
    }
}
