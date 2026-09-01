using iTextSharp.text;
using iTextSharp.text.pdf;
using MultiDbTablePdfCreate;
using SqlToPdfExport.services;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SqlToPdfExport
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            List<string> tableNames = DatabaseService.GetAllTableNames(ConfigReader.connectionString);
           PdfService.CreatePdfFromTables(
                ConfigReader.connectionString,
                tableNames,
                ConfigReader.pdfPath,
                ConfigReader.logoPath,
                ConfigReader.headerText
            );

            Console.WriteLine("PDF Generated Successfully!");
            Console.WriteLine("Program completed in: " + sw.Elapsed);
            Console.WriteLine("Milliseconds: " + sw.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
