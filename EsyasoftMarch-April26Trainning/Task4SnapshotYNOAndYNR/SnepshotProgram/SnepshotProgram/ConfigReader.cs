using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
public class ConfigReader
{
    public static string conStr => ConfigurationManager
                   .ConnectionStrings["db"].ConnectionString;
    public static int SleepTime => int.Parse(ConfigurationManager.AppSettings["SleepTime"]);

}


