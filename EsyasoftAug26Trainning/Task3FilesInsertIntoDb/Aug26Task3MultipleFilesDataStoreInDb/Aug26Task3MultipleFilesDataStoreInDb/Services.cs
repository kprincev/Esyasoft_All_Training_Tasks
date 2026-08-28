
using ClosedXML.Excel;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Text.Json;
using System.Xml.Linq;

namespace Aug26Task3MultipleFilesDataStoreInDb.Service
{
    public class Services
    {
        // CSV to DataTable
        public static DataTable CsvToDataTable(string filePath)
        {
            DataTable dt = new DataTable();

            using TextFieldParser parser = new TextFieldParser(filePath);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;

            string[] headers = parser.ReadFields();

            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                DataRow row = dt.NewRow();

                for (int i = 0; i < headers.Length; i++)
                {
                    row[i] = fields[i];
                }

                dt.Rows.Add(row);
            }

            return dt;
        }


        // XML to DataTable
        public static DataTable XmlToDataTable(string filePath)
        {
            DataTable dt = new DataTable();

            XDocument doc = XDocument.Load(filePath);

            IEnumerable<XElement> employees = doc.Root!.Elements("Employee");

            foreach (var element in employees.First().Elements())
            {
                dt.Columns.Add(element.Name.LocalName);
            }

            foreach (var employee in employees)
            {
                DataRow row = dt.NewRow();

                foreach (var element in employee.Elements())
                {
                    row[element.Name.LocalName] = element.Value;
                }

                dt.Rows.Add(row);
            }

            return dt;
        }


        // Excel to DataTable
        public static DataTable ExcelToDataTable(string filePath)
        {
            DataTable dt = new DataTable();

            using XLWorkbook workbook = new XLWorkbook(filePath);

            IXLWorksheet worksheet = workbook.Worksheet(1);

            bool firstRow = true;

            foreach (IXLRow row in worksheet.RowsUsed())
            {
                if (firstRow)
                {
                    foreach (IXLCell cell in row.CellsUsed())
                    {
                        dt.Columns.Add(cell.Value.ToString());
                    }

                    firstRow = false;
                }
                else
                {
                    DataRow dataRow = dt.NewRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dataRow[i] = row.Cell(i + 1).Value.ToString();
                    }

                    dt.Rows.Add(dataRow);
                }
            }

            return dt;
        }


        // JSON to DataTable
        public static DataTable JsonToDataTable(string filePath)
        {
            string json = File.ReadAllText(filePath);

            using JsonDocument doc = JsonDocument.Parse(json);

            JsonElement employees =
                doc.RootElement.GetProperty("Employees");

            DataTable dt = new DataTable();

            foreach (JsonProperty property in employees[0].EnumerateObject())
            {
                dt.Columns.Add(property.Name);
            }

            foreach (JsonElement employee in employees.EnumerateArray())
            {
                DataRow row = dt.NewRow();

                foreach (JsonProperty property in employee.EnumerateObject())
                {
                    row[property.Name] = property.Value.ToString();
                }

                dt.Rows.Add(row);
            }

            return dt;
        }


        // DataTable to JSON
        public static string DataTableToJson(DataTable dt)
        {
            var rows = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn column in dt.Columns)
                {
                    dict[column.ColumnName] =
                        row[column] == DBNull.Value ? null : row[column];
                }

                rows.Add(dict);
            }

            return JsonSerializer.Serialize(new {Employees = rows }, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}