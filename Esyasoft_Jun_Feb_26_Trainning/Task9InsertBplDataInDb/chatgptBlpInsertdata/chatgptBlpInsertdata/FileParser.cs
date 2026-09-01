using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using chatgptBlpInsertdata;
public static class FileParser
{
    public static DataTable Parse(string file)
    {
        DataTable table = MeterSchema.Create();

        // key = MeterId|IntervalDateTime
        Dictionary<string, DataRow> dedup = new Dictionary<string, DataRow>();

        var lines = File.ReadLines(file).Skip(1);

        // group by MeterId + FirstIntervalDateTime
        var groups = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Split('~'))
            .GroupBy(p => $"{p[6]}|{p[13]}");

        foreach (var group in groups)
        {
            var first = group.First();

            string meterId = first[6];

            DateTime baseTime = DateTime.ParseExact(
                first[13],
                "ddMMyyyyhhmmsstt",
                CultureInfo.InvariantCulture);

            int intervalMinutes = int.Parse(first[11]); // 15 / 30
            int count = int.Parse(first[12]);

            // Unit → Values
            Dictionary<string, List<decimal>> unitMap = new Dictionary<string, List<decimal>>();

            foreach (var line in group)
            {
                string unit = line[9]; // Units column
                unitMap[unit] = ValueExtractorr.Extract(line);
            }

            for (int i = 0; i < count; i++)
            {
                DateTime time = baseTime.AddMinutes(intervalMinutes * i);
                string key = meterId + "|" + time;

                DataRow row = table.NewRow();
                row["Meter_Id"] = meterId;
                row["IntervalDateTime"] = time;

                row["Avg_Voltage_V"] = unitMap.ContainsKey("Avg_Voltage (V)") ? unitMap["Avg_Voltage (V)"][i] : 0;
                row["BlkEngy_kWh"] = unitMap.ContainsKey("BlkEngy_I/F (kWh)") ? unitMap["BlkEngy_I/F (kWh)"][i] : 0;
                row["BlkEngy_kVAh"] = unitMap.ContainsKey("BlkEngy_I/F (kVAh)") ? unitMap["BlkEngy_I/F (kVAh)"][i] : 0;
                row["Avg_Current_A"] = unitMap.ContainsKey("Avg_Current (A)") ? unitMap["Avg_Current (A)"][i] : 0;

                // LATEST WINS
                if (dedup.ContainsKey(key))
                    dedup[key].ItemArray = row.ItemArray;
                else
                {
                    dedup[key] = row;
                    table.Rows.Add(row);
                }
            }
        }

        return table;
    }
}
