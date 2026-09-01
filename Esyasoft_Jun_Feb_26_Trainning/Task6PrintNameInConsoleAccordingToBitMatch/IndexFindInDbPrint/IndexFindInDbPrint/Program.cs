using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexFindInDbPrint
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string str = "";
            string bits = ConfigurationManager.AppSettings["strbit"];
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == '1')
                {
                    DataBaseService ob = new DataBaseService();
                    string result=ob.DataBaseindex(i);
                       
                    str=str+ result+",";
                    
                }
            }
           
            Console.WriteLine( str.Trim());
        }
    }
}
