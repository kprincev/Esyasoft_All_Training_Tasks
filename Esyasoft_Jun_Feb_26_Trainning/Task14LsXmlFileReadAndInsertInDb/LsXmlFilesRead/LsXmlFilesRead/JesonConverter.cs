using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsXmlFilesRead
{
   public  class JesonConverter
    {
        public static string DataTableToJson(DataTable table)
        {
            return JsonConvert.SerializeObject(
                table,
                Newtonsoft.Json.Formatting.Indented   // readable JSON
            );
        }
    }
}
