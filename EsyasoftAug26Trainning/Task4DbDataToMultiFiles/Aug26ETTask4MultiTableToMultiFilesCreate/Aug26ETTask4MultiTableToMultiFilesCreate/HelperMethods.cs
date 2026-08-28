using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aug26ETTask4MultiTableToMultiFilesCreate
{
    public  class HelperMethods
    {
       public static List<Dictionary<string, string>> GetRowsDynamically(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            var firstRow = new Dictionary<string, string>();

            return ExpandJson(document.RootElement, new List<Dictionary<string, string>> { firstRow }, "");
        }
        static List<Dictionary<string, string>> ExpandJson(JsonElement element, List<Dictionary<string, string>> inputRows, string currentPath)
        {
            // JSON Object
            if (element.ValueKind == JsonValueKind.Object)
            {
                List<Dictionary<string, string>> rows = inputRows;

                foreach (JsonProperty property in element.EnumerateObject())
                {
                    string propertyPath = string.IsNullOrEmpty(currentPath)
                        ? property.Name
                        : currentPath + "." + property.Name;

                    // Simple value: string, number, boolean, null
                    if (property.Value.ValueKind != JsonValueKind.Object &&
                        property.Value.ValueKind != JsonValueKind.Array)
                    {
                        foreach (Dictionary<string, string> row in rows)
                        {
                            row[propertyPath] =
                                property.Value.ValueKind == JsonValueKind.Null
                                    ? ""
                                    : property.Value.ToString();
                        }
                    }
                    else
                    {
                        rows = ExpandJson(property.Value, rows, propertyPath);
                    }
                }

                return rows;
            }

            // JSON Array
            if (element.ValueKind == JsonValueKind.Array)
            {
                var expandedRows = new List<Dictionary<string, string>>();

                foreach (Dictionary<string, string> row in inputRows)
                {
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        // Current row ki copy banegi
                        var copiedRow = new Dictionary<string, string>(row);

                        // Array item ko process karega
                        expandedRows.AddRange(
                            ExpandJson(
                                item,
                                new List<Dictionary<string, string>> { copiedRow },
                                currentPath
                            )
                        );
                    }
                }

                return expandedRows;
            }


            foreach (Dictionary<string, string> row in inputRows)
            {
                row[currentPath] = element.ToString();
            }

            return inputRows;
        }
       public static Dictionary<string, string> CreateDisplayHeaders(List<string> jsonPaths)
        {
            var temporaryHeaders = new Dictionary<string, string>();

            var groups = jsonPaths.GroupBy(path => path.Split('.').Last());

            foreach (var group in groups)
            {
                // Agar same final property name sirf ek baar hai
                if (group.Count() == 1)
                {
                    string path = group.First();
                    temporaryHeaders[path] = group.Key;
                }
                else
                {

                    foreach (string path in group)
                    {
                        string[] parts = path.Split('.');
                        string parentName = parts.Length > 1
                            ? GetSingularName(parts[^2])
                            : "";

                        temporaryHeaders[path] =
                            char.ToUpper(parentName[0]) + parentName.Substring(1) +
                            char.ToUpper(group.Key[0]) + group.Key.Substring(1);
                    }
                }
            }

            // Same header name agar phir bhi aaye to unique bana dega
            var finalHeaders = new Dictionary<string, string>();
            var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string path in jsonPaths)
            {
                string header = temporaryHeaders[path];
                string uniqueHeader = header;
                int number = 2;

                while (usedNames.Contains(uniqueHeader))
                {
                    uniqueHeader = header + number;
                    number++;
                }

                usedNames.Add(uniqueHeader);
                finalHeaders[path] = uniqueHeader;
            }

            return finalHeaders;
        }
        public static string GetSingularName(string arrayName)
        {
            if (arrayName.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
            {
                return arrayName.Substring(0, arrayName.Length - 3) + "y";
            }

            if (arrayName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return arrayName.Substring(0, arrayName.Length - 1);
            }

            return "item";
        }
    }
}
