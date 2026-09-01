using System;
using System.Collections.Generic;


namespace InsertBplData
{
    public  class SplitService
    {
        public static List<MeterRecord> SplitServe(String rawData)
        {
            List<MeterRecord> records = new List<MeterRecord>();

            string[] lines = rawData .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

         
            for (int l = 1; l < lines.Length; l++)
            {
                string[] parts = lines[l].Trim().Split('~');

                string meterId = parts[6];
                string units = parts[9];
                string firstIntervalStr = parts[13];
                int interval=int.Parse(parts[11]);

                DateTime firstIntervalDateTime = DateTime.ParseExact(firstIntervalStr,"ddMMyyyyhhmmsstt", System.Globalization.CultureInfo.InvariantCulture);


                List<decimal> values = new List<decimal>();

                for (int i = 14; i < parts.Length; i++)
                {
                    // normal case
                    if (decimal.TryParse(parts[i], out decimal v))
                    {
                        values.Add(v);
                        continue;
                    }

                    // A~value case
                    if (parts[i] == "A"  && i + 1 < parts.Length && decimal.TryParse(parts[i + 1], out decimal av))
                    {
                        values.Add(av);
                        i++;
                    }
                }

                records.Add(new MeterRecord
                {
                    MeterId = meterId,
                    Units = units,
                    FirstIntervalDateTime = firstIntervalDateTime,
                    Data = values,
                    interval=interval
                });
            }
            return records;
        }
    }
}
