using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;

namespace RabbitFilePipeline.Processor
{
    public static class PendingProcessor
    {
        static readonly object _errorLogLock = new object();

        public static void Run()
        {
            string pending = ConfigurationManager.AppSettings["PendingFolder"];
            string process = ConfigurationManager.AppSettings["ProcessFolder"];
            string error = ConfigurationManager.AppSettings["ErrorFolder"];
            string dberror = ConfigurationManager.AppSettings["DBErrorFolder"];

            Directory.CreateDirectory(pending);
            Directory.CreateDirectory(process);
            Directory.CreateDirectory(error);
            Directory.CreateDirectory(dberror);

            var files = Directory.GetFiles(pending);
            if (files.Length == 0) return;

            Parallel.ForEach(files, file =>
            {
                try
                {
                    InsertIntoDb(file);
                    Move(file, process);
                }
                catch (SqlException ex)
                {
                    LogError(error, file, ex);
                    Move(file, dberror);
                }
                catch (Exception ex)
                {
                    LogError(error, file, ex);
                    Move(file, error);
                }
            });
        }

        static void InsertIntoDb(string file)
        {
            string ext = Path.GetExtension(file).ToLower();

            if (ext == ".json")
                ProcessJson(file);
            else if (ext == ".csv")
                ProcessCsv(file);
            else if (ext == ".xml")
                ProcessXml(file);
            else
                throw new Exception("Unsupported file type");
        }

        // JSON
        static void ProcessJson(string file)
        {
            string json = File.ReadAllText(file).Trim();

            if (!json.StartsWith("["))
                json = "[" + json + "]";

            InsertJson(json);
        }

        // CSV
        static void ProcessCsv(string file)
        {
            var lines = File.ReadAllLines(file);

            var headers = lines[0].Split(',');

            var list = new List<Dictionary<string, string>>();

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = line.Split(',');

                var obj = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    obj[headers[i].Trim()] = values[i].Trim();
                }

                list.Add(obj);
            }

            string json = JsonConvert.SerializeObject(list);

            InsertJson(json);
        }

        // XML
        static void ProcessXml(string file)
        {
            var doc = XDocument.Load(file);

            var list = new List<Dictionary<string, string>>();

            foreach (var reading in doc.Descendants("Reading"))
            {
                var obj = new Dictionary<string, string>();

                foreach (var element in reading.Elements())
                {
                    obj[element.Name.LocalName] = element.Value;
                }

                list.Add(obj);
            }

            string json = JsonConvert.SerializeObject(list);

            InsertJson(json);
        }
        // SEND JSON TO SQL
        static void InsertJson(string json)
        {
            var csObj = ConfigurationManager.ConnectionStrings["db"];

            if (csObj == null)
                throw new Exception("Connection string not found");

            using (var con = new SqlConnection(csObj.ConnectionString))
            using (var cmd = new SqlCommand("usp_InsertMeterReadingJson", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@json", SqlDbType.NVarChar).Value = json;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        static void LogError(string errorFolder, string file, Exception ex)
        {
            string logPath = Path.Combine(errorFolder, "error.log");

            lock (_errorLogLock)
            {
                File.AppendAllText(
                    logPath,
                    DateTime.Now + " | " + file + Environment.NewLine +
                    ex + Environment.NewLine +
                    "--------------------------------------------" + Environment.NewLine
                );
            }
        }

        static void Move(string src, string destFolder)
        {
            Directory.CreateDirectory(destFolder);

            string dest = Path.Combine(destFolder, Path.GetFileName(src));
            if (File.Exists(dest)) File.Delete(dest);

            File.Move(src, dest);
        }
    }
}