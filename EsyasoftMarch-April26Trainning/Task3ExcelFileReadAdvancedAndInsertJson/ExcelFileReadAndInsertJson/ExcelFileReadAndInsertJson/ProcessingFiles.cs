using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ExcelFileReadAndInsertJson
{
    public class ProcessingFiles
    {
        public static async Task ProcessFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            Console.WriteLine($"Processing: {fileName}");

            Dictionary<string, object> sheetsData = null;
            string json = "";
            try
            {
                sheetsData = new Dictionary<string, object>();

                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    foreach (IXLWorksheet sheet in workbook.Worksheets)
                    {
                        var sheetRows = new List<Dictionary<string, object>>();

                        IXLRow firstRow = sheet.FirstRowUsed();
                    //    if (firstRow == null) continue;

                        List<string> headers = firstRow.Cells().Select(c => c.GetValue<string>()).ToList();

                        foreach (IXLRow row in sheet.RowsUsed().Skip(1))
                        {
                            var rowData = new Dictionary<string, object>();
                            int colIndex = 0;

                            foreach (IXLCell cell in row.Cells(1, headers.Count))
                            {
                                object value = cell.GetValue<object>();
                                rowData[headers[colIndex]] = value;
                                colIndex++;
                            }

                            sheetRows.Add(rowData);
                        }

                        sheetsData[sheet.Name] = sheetRows;
                    }
                }

                json = JsonConvert.SerializeObject(sheetsData);
                JsonObject.Parse(json);
                Console.WriteLine($"JSON Created: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read Error: {fileName} - {ex.Message}");
                Console.WriteLine($"Moving to Read Error Folder: {fileName}");
               Console.WriteLine("File Move To => "+ MoveFile(filePath, ConfigReader.ReadErrorFolder));
                return;
            }

            try
            {
                DatabaseService.InsertIntoDatabase(fileName, json);

               Console.WriteLine($"DB Inserted: {fileName}");
                Console.WriteLine($"Moving to Processed Folder: {fileName}");
                Console.WriteLine("File Move To => "+MoveFile(filePath, ConfigReader.ProcessedFolder));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Error: {fileName} - {ex.Message}");
                Console.WriteLine($"Moving to DB Error Folder: {fileName}");
                Console.WriteLine("File Move To => "+MoveFile(filePath, ConfigReader.DbErrorFolder));
            }
        }

        static string MoveFile(string sourcePath, string destinationFolder)
        {
            try
            {
                string fileName = Path.GetFileName(sourcePath);
                string destPath = Path.Combine(destinationFolder, fileName);

                if (File.Exists(destPath))
                {
                    string newName = Path.GetFileNameWithoutExtension(fileName)
                                    + "_" + DateTime.Now.Ticks
                                    + Path.GetExtension(fileName);

                    destPath = Path.Combine(destinationFolder, newName);
                }

                File.Move(sourcePath, destPath);
                return destPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Move Error: {ex.Message}");
                return sourcePath;
            }
        }
    }
}
