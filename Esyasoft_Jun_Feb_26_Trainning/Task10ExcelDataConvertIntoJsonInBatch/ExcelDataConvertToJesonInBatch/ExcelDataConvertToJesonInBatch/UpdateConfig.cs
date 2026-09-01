using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDataConvertToJesonInBatch
{
    public  class UpdateConfig
    {
        public static void UpdateAppConfig(int remaining,string name)
        {
            try
            {
               
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[name] != null)
                    config.AppSettings.Settings[name].Value = remaining.ToString();
                else
                    config.AppSettings.Settings.Add(name, remaining.ToString());

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string sourceConfigPath = Path.Combine(projectDirectory, "App.config");

                if (File.Exists(sourceConfigPath))
                {
                    
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.Load(sourceConfigPath);

                    var node = xmlDoc.SelectSingleNode($"//add[@key='{name}']");
                    if (node != null)
                    {
                        node.Attributes["value"].Value = remaining.ToString();
                        xmlDoc.Save(sourceConfigPath);
                        Console.WriteLine("Source App.config updated successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating config: " + ex.Message);
            }
        }
    }
}

