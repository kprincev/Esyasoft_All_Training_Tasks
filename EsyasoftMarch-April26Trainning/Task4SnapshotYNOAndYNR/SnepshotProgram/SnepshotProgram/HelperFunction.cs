using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnepshotProgram
{
    public class HelperFunction
    {
        public static string ConvertToJson(List<DataRow> rows)
        {
            var list = rows.Select(r => r.Table.Columns
                .Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => r[c]));

            return JsonConvert.SerializeObject(list);
        }
        public static Dictionary<string, object> RowToDict(DataRow row)
        {
            return row.Table.Columns
                .Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => row[c]);
        }

    }
}
