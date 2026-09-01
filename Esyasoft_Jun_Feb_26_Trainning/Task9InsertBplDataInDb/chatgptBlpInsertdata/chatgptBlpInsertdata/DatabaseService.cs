using chatgptBlpInsertdata;
using System.Data;
using System.Data.SqlClient;

public static class DatabaseService
{
    public static void BulkInsert(DataTable table)
    {
        using (SqlConnection conn =new SqlConnection(AppConfig.ConnectionString))
        {

            conn.Open();

            using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
            {
                bulk.DestinationTableName = "MeterIntervalData";
                bulk.BatchSize = 5000;
                bulk.WriteToServer(table);
            }

        }

    }
}
