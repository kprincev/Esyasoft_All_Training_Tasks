using System;
using System.Globalization;
using System.IO;

public static class FileValidator
{
    public static bool IsValid(string file)
    {
        try
        {
            using (var reader = new StreamReader(file))
            {
                string header = reader.ReadLine();
                string data = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Record Type"))
                    return false;

                var parts = data.Split('~');
                if (parts.Length < 14 || string.IsNullOrWhiteSpace(parts[6]))
                    return false;

                DateTime.ParseExact(
                    parts[13],
                    "ddMMyyyyhhmmsstt",
                    CultureInfo.InvariantCulture);

                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
