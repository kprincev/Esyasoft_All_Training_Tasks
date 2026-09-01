using iTextSharp.text;
using iTextSharp.text.pdf;
using SqlToPdfExport.services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiDbTablePdfCreate
{


     public class PdfService

      {


          public static void CreatePdfFromTables(
               string connStr,
               List<string> tableNames,
               string pdfPath,
               string logoPath,
               string headerText)
          {


              if (ConfigReader.tableCount > tableNames.Count)
                  throw new Exception("Table count exceeds available tables.");

              Document document = new Document(PageSize.A4, 20, 20, 30, 30);
            PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));


              document.Open();
              AddHeader(document, logoPath, headerText);

              object pdfLock = new object();
              List<Thread> threads = new List<Thread>();

              SemaphoreSlim semaphore =  new SemaphoreSlim(ConfigReader.maxThreads);

              for (int i = 0; i < ConfigReader.tableCount; i++)
              {
                  string tableName = tableNames[i];

                  Thread t = new Thread(() =>
                  {
                      semaphore.Wait();
                      try
                      {
                          DataTable dt =
                              DatabaseService.GetTableData(connStr, tableName);

                          PdfPTable pdfTable = ConvertToPdfTable(dt);

                          lock (pdfLock)
                          {
                              document.Add(new Paragraph("\n"));
                              document.Add(new Paragraph(
                                  "Table : " + tableName,
                                  FontFactory.GetFont(
                                      FontFactory.HELVETICA_BOLD, 13)));

                              document.Add(new Paragraph("\n"));
                              document.Add(pdfTable);
                          }
                      }
                      finally
                      {
                          semaphore.Release();
                      }
                  });

                  threads.Add(t);
                  t.Start();
              }


              foreach (var t in threads)
                  t.Join();

              document.Close();
          }

          static void AddHeader(Document doc, string logoPath, string text)
          {
              PdfPTable headerTable = new PdfPTable(3);
              headerTable.WidthPercentage = 100;
              headerTable.SetWidths(new float[] { 3,1 ,2 });

              // Logo
              iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
              logo.ScaleToFit(70, 70);
              PdfPCell logoCell = new PdfPCell(logo);
              logoCell.Border = Rectangle.NO_BORDER;

              // Header Text
              PdfPCell textCell = new PdfPCell(
                  new Phrase(text,
                  FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18))
              );
              textCell.VerticalAlignment = Element.ALIGN_MIDDLE;
              textCell.Border = Rectangle.NO_BORDER;

              PdfPCell date = new PdfPCell(
                 new Phrase(DateTime.Now.ToString("dd-MMM-yyyy"),
                 FontFactory.GetFont(FontFactory.HELVETICA, 13))
             );

              textCell.VerticalAlignment = Element.ALIGN_RIGHT;
              date.Border=Rectangle.NO_BORDER;

              headerTable.AddCell(textCell);
              headerTable.AddCell(logoCell);
              headerTable.AddCell(date);
              doc.Add(headerTable);
          }
          static PdfPTable ConvertToPdfTable(DataTable dt)
          {
              PdfPTable table = new PdfPTable(dt.Columns.Count);
              table.WidthPercentage = 100;


              foreach (DataColumn col in dt.Columns)
              {
                  PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName));
                  cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                  cell.HorizontalAlignment = Element.ALIGN_CENTER;
                  table.AddCell(cell);
              }


              foreach (DataRow row in dt.Rows)
              {
                  foreach (var item in row.ItemArray)
                  {
                      table.AddCell(item.ToString());
                  }
              }

              return table;
          }
      }


    /*
    public class PdfService

    {
        public static void CreatePdfFromTables(
             string connStr,
             List<string> tableNames,
             string pdfPath,
             string logoPath,
             string headerText)
        {


            if (ConfigReader.tableCount > tableNames.Count)
                throw new Exception("Table count exceeds available tables.");

            Document document = new Document(PageSize.A4, 20, 20, 30, 30);
            PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));


            document.Open();
            AddHeader(document, logoPath, headerText);


            for (int i = 0; i < ConfigReader.tableCount; i++)
            {
                string tableName = tableNames[i]; 
                        DataTable dt =
                            DatabaseService.GetTableData(connStr, tableName);

                        PdfPTable pdfTable = ConvertToPdfTable(dt);             
                            document.Add(new Paragraph("\n"));
                            document.Add(new Paragraph(
                                "Table : " + tableName,
                                FontFactory.GetFont(
                                    FontFactory.HELVETICA_BOLD, 13)));

                            document.Add(new Paragraph("\n"));
                            document.Add(pdfTable);    
            }
            document.Close();
        }

        static void AddHeader(Document doc, string logoPath, string text)
        {
            PdfPTable headerTable = new PdfPTable(3);
            headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new float[] { 3, 1, 2 });

            // Logo
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleToFit(70, 70);
            PdfPCell logoCell = new PdfPCell(logo);
            logoCell.Border = Rectangle.NO_BORDER;

            // Header Text
            PdfPCell textCell = new PdfPCell(
                new Phrase(text,
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18))
            );
            textCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            textCell.Border = Rectangle.NO_BORDER;

            PdfPCell date = new PdfPCell(
               new Phrase(DateTime.Now.ToString("dd-MMM-yyyy"),
               FontFactory.GetFont(FontFactory.HELVETICA, 13))
           );

            textCell.VerticalAlignment = Element.ALIGN_RIGHT;
            date.Border = Rectangle.NO_BORDER;

            headerTable.AddCell(textCell);
            headerTable.AddCell(logoCell);
            headerTable.AddCell(date);
            doc.Add(headerTable);
        }
        static PdfPTable ConvertToPdfTable(DataTable dt)
        {
            PdfPTable table = new PdfPTable(dt.Columns.Count);
            table.WidthPercentage = 100;


            foreach (DataColumn col in dt.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
            }


            foreach (DataRow row in dt.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    table.AddCell(item.ToString());
                }
            }

            return table;
        }
    }
     */
}





