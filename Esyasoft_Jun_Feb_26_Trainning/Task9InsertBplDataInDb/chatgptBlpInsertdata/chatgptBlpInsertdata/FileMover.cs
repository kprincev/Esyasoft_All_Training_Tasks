using System;
using System.IO;

public static class FileMover
{
    public static string Move(string src, string destFolder)
    {
        string dest = Path.Combine(destFolder, Path.GetFileName(src));
        if (File.Exists(dest))
        {
            File.Delete(dest);
        }
        File.Move(src, dest);
        return dest;
    }

    public static void SafeMove(string src, string destFolder)
    {
        if (File.Exists(src))
            Move(src, destFolder);
    }

    public static string MoveToProcessedWithDate(string sourceFile, string processedRoot)
    {
        // 👉 Aaj ki date ka folder
        string dateFolderName = DateTime.Today.ToString("yyyy-MM-dd");

        // 👉 Full path: Processed/yyyy-MM-dd
        string datedFolderPath = Path.Combine(processedRoot, dateFolderName);

        // 👉 Folder exist nahi karta to create
        Directory.CreateDirectory(datedFolderPath);

        // 👉 Destination file path
        string destinationFile = Path.Combine(
            datedFolderPath,
            Path.GetFileName(sourceFile));

        // 👉 Move (overwrite allowed)
        if (File.Exists(destinationFile))
        {
            File.Delete(destinationFile);
        }
        File.Move(sourceFile, destinationFile);

        return destinationFile;
    }
}
