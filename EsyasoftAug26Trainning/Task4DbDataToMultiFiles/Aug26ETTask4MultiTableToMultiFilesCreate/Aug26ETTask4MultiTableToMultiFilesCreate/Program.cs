using Aug26ETTask4MultiTableToMultiFilesCreate;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using ClosedXML.Excel;
using System.Text.Json;
using System.Xml.Linq;

class Program
{
    public static void Main(string[] args)
    {
        ConfigReader.config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();
    
        string json;

        using (SqlConnection connection = new SqlConnection(ConfigReader.ConnectionString))
        using (SqlCommand command = new SqlCommand(ConfigReader.SpName, connection))
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;

            connection.Open();

            object? result = command.ExecuteScalar();

            json = result?.ToString() ?? throw new Exception("JSON data not found.");
        }

        SaveJsonInAllFormats(json, ConfigReader.SaveLocation);

        static void SaveJsonInAllFormats(string json, string folderPath)
        {
            Directory.CreateDirectory(folderPath);

            // JSON validate karna
            using JsonDocument document = JsonDocument.Parse(json);
            Console.WriteLine("Json File Conversion start");
            // 1. Original JSON file
            File.WriteAllText(Path.Combine(folderPath, "company.json"),json,Encoding.UTF8 );

            Console.WriteLine("xml File Conversion start");
            // 2. XML file
            XElement xmlRoot = JsonToXml("company", document.RootElement);

            new XDocument(   new XDeclaration("1.0", "utf-8", "yes"), xmlRoot ).Save(Path.Combine(folderPath, "company.xml"));
            Console.WriteLine("csv File Conversion start");
            // 4. CSV file
            CreateCsvFromJson(json, Path.Combine(folderPath, "company.csv"));
            Console.WriteLine("excel File Conversion start");
            CreateExcelFromJson(json, Path.Combine(folderPath, "company.xlsx"));
        }

        static XElement JsonToXml(string name, JsonElement element)
        {
            XElement xml = new XElement(name);

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    xml.Add(JsonToXml(property.Name, property.Value));
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                string itemName = HelperMethods.GetSingularName(name);

                foreach (JsonElement item in element.EnumerateArray())
                {
                    xml.Add(JsonToXml(itemName, item));
                }
            }
            else if (element.ValueKind != JsonValueKind.Null)
            {
                xml.Value = element.ToString();
            }

            return xml;
        }



       
        static void CreateCsvFromJson(string json, string filePath)
        {
            List<Dictionary<string, string>> rows = HelperMethods.GetRowsDynamically(json);

            if (rows.Count == 0)
                return;

            List<string> jsonPaths = rows[0].Keys.ToList();

            Dictionary<string, string> displayHeaders = HelperMethods.CreateDisplayHeaders(jsonPaths);
            StringBuilder csv = new StringBuilder();

            // Header
            csv.AppendLine(string.Join(",",jsonPaths.Select(path => $"\"{displayHeaders[path]}\"")));

            // Rows
            foreach (Dictionary<string, string> row in rows)
            {
                csv.AppendLine(string.Join(",", jsonPaths.Select(path => $"\"{row[path].Replace("\"", "\"\"")}\""  )  ));
            }

            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }
        static void CreateExcelFromJson(string json, string filePath)
        {
            List<Dictionary<string, string>> rows = HelperMethods.GetRowsDynamically(json);

            if (rows.Count == 0)
                return;

            // Sab rows se dynamic JSON paths/columns nikalega
            List<string> jsonPaths = rows
                .SelectMany(row => row.Keys)
                .Distinct()
                .ToList();

            Dictionary<string, string> displayHeaders = HelperMethods.CreateDisplayHeaders(jsonPaths);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Company Data");

            // Header row
            for (int columnIndex = 0; columnIndex < jsonPaths.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value =  displayHeaders[jsonPaths[columnIndex]];
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < jsonPaths.Count; columnIndex++)
                {
                    string jsonPath = jsonPaths[columnIndex];

                    worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = rows[rowIndex].TryGetValue(jsonPath, out string? value)  ? value : "";
                }
            }

            // Excel table create
            worksheet.Range(1, 1, rows.Count + 1, jsonPaths.Count).CreateTable();

            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(1);

            workbook.SaveAs(filePath);
        }
       
    }
}