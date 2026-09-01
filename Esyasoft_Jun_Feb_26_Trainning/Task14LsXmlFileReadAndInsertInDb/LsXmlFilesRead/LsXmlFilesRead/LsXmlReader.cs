using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;

public static class LsXmlReader
{
    public static DataTable ReadLsXml(string xml)
    {
        XDocument doc = XDocument.Parse(xml);
        XNamespace ns = "http://www.emeter.com/energyip/amiinterface";

        DataTable table = new DataTable();
        table.Columns.Add("MeterSerialNumber", typeof(string));
        table.Columns.Add("IntervalTime", typeof(DateTime));

        var meterReading = doc.Descendants(ns + "MeterReading").First();
    

        string currentMeterSerial = null;
        List<XElement> currentBlocks = new List<XElement>();

        Dictionary<string, string> rtMap = null;

        foreach (var node in meterReading.Elements())
        {
            // 🔁 New Meter detected
            if (node.Name == ns + "Meter")
            {
                // process previous meter
                if (currentMeterSerial != null && currentBlocks.Any())
                {
                    ProcessMeter(table, currentMeterSerial, currentBlocks, ns, ref rtMap);
                    currentBlocks.Clear();
                }

                currentMeterSerial = node.Element(ns + "serialNumber")?.Value;
            }

            // collect IntervalBlocks
            if (node.Name == ns + "IntervalBlock")
            {
                currentBlocks.Add(node);
            }
        }

        // 🔚 process last meter
        if (currentMeterSerial != null && currentBlocks.Any())
        {
            ProcessMeter(table, currentMeterSerial, currentBlocks, ns, ref rtMap);
        }

        return table;
    }

    

    private static void ProcessMeter(
        DataTable table,
        string meterSerial,
        List<XElement> intervalBlocks,
        XNamespace ns,
        ref Dictionary<string, string> rtMap)
    {
        // Build readingType → column map once
        if (rtMap == null)
        {
            rtMap = intervalBlocks.ToDictionary(
                b => b.Element(ns + "readingTypeId").Value,
                b =>
                {
                    string rt = b.Element(ns + "readingTypeId").Value;
                    string rtWithoutColon = rt.Split(':')[0].Replace(".", "_");

                    return ConfigurationManager.AppSettings[rtWithoutColon];
                }
            );

            foreach (var col in rtMap.Values)
            {
                if (!table.Columns.Contains(col))
                    table.Columns.Add(col, typeof(decimal));
            }
        }

        // Time slots from first block
        var timeSlots = intervalBlocks
            .First()
            .Elements(ns + "IReading")
            .Select(r => DateTime.Parse(r.Element(ns + "endTime").Value))
            .ToList();

        foreach (var time in timeSlots)
        {
            DataRow row = table.NewRow();
            row["MeterSerialNumber"] = meterSerial;
            row["IntervalTime"] = time;

            foreach (var block in intervalBlocks)
            {
                string rt = block.Element(ns + "readingTypeId").Value;
                string colName = rtMap[rt];

                var reading = block.Elements(ns + "IReading")
                    .First(r => DateTime.Parse(r.Element(ns + "endTime").Value) == time);

                row[colName] = decimal.Parse(reading.Element(ns + "value").Value);
            }

            table.Rows.Add(row);
        }
    }
}
