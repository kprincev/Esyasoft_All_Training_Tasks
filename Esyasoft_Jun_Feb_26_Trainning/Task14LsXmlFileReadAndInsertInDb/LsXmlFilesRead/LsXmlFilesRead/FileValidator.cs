using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LsXmlFilesRead
{
    public  class FileValidator
    {
        public static bool IsValid(string xml )
        {
           
            XDocument doc = XDocument.Parse(xml);
            XNamespace ns = "http://www.emeter.com/energyip/amiinterface";
            try
            {
                if (!doc.Descendants(ns + "MeterReading").Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"XML parsing error: {ex.Message}");
                return false;
            }

            
        }
    }
}
