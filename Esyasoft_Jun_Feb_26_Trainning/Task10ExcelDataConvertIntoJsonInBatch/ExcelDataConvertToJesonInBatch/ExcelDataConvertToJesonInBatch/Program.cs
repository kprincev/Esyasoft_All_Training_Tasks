using ClosedXML.Excel;
using ExcelDataConvertToJesonInBatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ExcelToJsonBatch
{
    class Program
    {
        static void Main(string[] args)
        {


            ConvertJeson.ProcessExcelWithResume(ConfigReader.filePath, ConfigReader.batchsize);

            Console.WriteLine("\nProcessing Complete! Press any key to exit.");
            Console.ReadKey();
        }
    }

       
        
}