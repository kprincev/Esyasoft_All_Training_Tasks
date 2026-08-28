using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Task6ReadXmlFileRealWorkingDataToJsonInDb
{
    public class XmlFileReadingService
    {
        public static string IpFileReadMethod(string XmlFile)
        {
            FileInfo fileInfo = new FileInfo(XmlFile);
            if (fileInfo.Length == 0)
            {
                throw new Exception("File blank hai (0 Bytes size). Skipping processing.");
            }

            // 2. Read File Safely using XDocument.Load (Encoding / BOM Issues Fixes)
            XDocument doc;
            using (var stream = File.OpenRead(XmlFile))
            {
                doc = XDocument.Load(stream);
            }
            var paramMapping = ConfigReader.config.GetSection("Instantaneous Parameter")
                         .Get<Dictionary<string, string>>();
         

            XNamespace headerNs = "http://iec.ch/TC57/2011/schema/message";
            XNamespace meterNs = "http://iec.ch/TC57/2011/MeterReadings#";

            // Global CorrelationID 
            string globalJobName = doc.Descendants(headerNs + "CorrelationID").FirstOrDefault()?.Value ?? "";

            var finalMetersList = new List<Dictionary<string, object>>();

            // 2. Multple <MeterReading> nodes par Loop
            var meterReadingsList = doc.Descendants(meterNs + "MeterReading");

            foreach (var meterReadingNode in meterReadingsList)
            {
                // Individual Meter Serial Number (MSN)
                string currentMeterSerialNo = meterReadingNode.Descendants(meterNs + "Meter")
                                                              .Descendants(meterNs + "name")
                                                              .FirstOrDefault()?.Value ?? "";

                var meterObj = new Dictionary<string, object>
            {
                { "meterserialno", currentMeterSerialNo },
                { "jobname", globalJobName }
            };
 
                var allReadingsForThisMeter = meterReadingNode.Elements(meterNs + "Readings").ToList();
                var firstReadingNode = allReadingsForThisMeter.FirstOrDefault();

                string firstTimeStampRaw = firstReadingNode?.Element(meterNs + "timeStamp")?.Value ?? "";
                string formattedFirstTimeStamp = FormatDateTime(firstTimeStampRaw);

                bool isFirstReadingProcessed = false;

                // Current Meter  Readings Loop
                foreach (var reading in allReadingsForThisMeter)
                {
                    string rawValue = reading.Element(meterNs + "value")?.Value ?? "";
                    string readingTypeRef = reading.Element(meterNs + "ReadingType")?.Attribute("ref")?.Value ?? "";

                    if (string.IsNullOrEmpty(readingTypeRef) || !paramMapping.ContainsKey(readingTypeRef))
                    {
                        throw new KeyNotFoundException($"ReadingType ref '{readingTypeRef}' not avaliable in appsettings Instantaneous Parameter ");
                    }

                    if (paramMapping != null && paramMapping.ContainsKey(readingTypeRef))
                    {
                        string mappedCode = paramMapping[readingTypeRef];
                        string formattedCode = mappedCode.Replace('.', '_');

                        string jsonKey = "";
                        string finalValue = rawValue;

                        // Date Check for Suffix (_A5 vs _A2)
                        if (DateTime.TryParse(rawValue, out _) && rawValue.Contains("T"))
                        {
                            jsonKey = $"Data_{formattedCode}_A5";
                            finalValue = FormatDateTime(rawValue);
                        }
                        else
                        {
                            jsonKey = $"Data_{formattedCode}_A2";
                        }

                        if (!meterObj.ContainsKey(jsonKey))
                        {
                            meterObj.Add(jsonKey, finalValue);
                        }

                        // Top level timeStamp placement 
                        if (!isFirstReadingProcessed)
                        {
                            meterObj.Add("timeStamp", formattedFirstTimeStamp);
                            isFirstReadingProcessed = true;
                        }
                    }
                }

                
                finalMetersList.Add(meterObj);
            }

            // 3. Final Formatted Multi-Meter JSON Output
            string jsonOutput = JsonConvert.SerializeObject(finalMetersList, Formatting.Indented);
            return jsonOutput;     
        }

        private static string FormatDateTime(string inputDate)
        {
            if (DateTime.TryParse(inputDate, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return inputDate;
        }
    }
    
}
