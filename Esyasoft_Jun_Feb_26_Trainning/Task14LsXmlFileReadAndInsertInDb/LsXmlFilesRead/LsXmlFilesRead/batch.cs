using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsXmlFilesRead
{
    public class batch
    {
        public static void Start()
        {
            while (true)
            {
                var files = Directory.GetFiles(ConfigReader.Pending, "*.*")
                                     .Take(10)
                                     .ToList();

                if (files.Count == 0)
                    break;

                foreach (var file in files)
                {
                    ProcessSingleFile(file);
                }
            }
        }
        private static void ProcessSingleFile(string file)
        {
            string xml = File.ReadAllText(file);
            try
            {
                if (!FileValidator.IsValid(xml))
                {
                    FileMover.Move(file, ConfigReader.ReadError);
                    return;
                }

                DataTable dt = LsXmlReader.ReadLsXml(xml);
                string json = JesonConverter.DataTableToJson(dt); 
        
                DatabaseServices.InsertJsonToDb(json, ConfigReader.constr);
                FileMover.Move(file, ConfigReader.Processed);
            }
            catch (IOException)
            {
                FileMover.SafeMove(file, ConfigReader.ReadError);
            }
            
        }
    }
}
