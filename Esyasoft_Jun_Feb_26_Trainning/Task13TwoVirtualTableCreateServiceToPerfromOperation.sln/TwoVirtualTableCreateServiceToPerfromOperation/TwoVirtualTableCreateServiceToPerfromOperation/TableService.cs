using System;
using System.Data;
using System.Data.SqlClient;

namespace TwoVirtualTableCreateServiceToPerfromOperation
{
    public class TableService
    {
        public static void DataExchager()
        {
            while (true)
            {
                // ---------------- SOURCE CONNECTION ----------------
                using (SqlConnection sourceConn = new SqlConnection(ConfigReader.sourceConnStr))
                {
                    sourceConn.Open();

                    DataTable sourceData = new DataTable();

                    using (SqlCommand fetchCmd = new SqlCommand("DataSelect", sourceConn))
                    {
                        fetchCmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(fetchCmd))
                        {
                            da.Fill(sourceData);
                        }
                    }

                    // NO MORE DATA
                    if (sourceData.Rows.Count == 0)
                    {
                        Console.WriteLine("✅ All data processed. No records left.");
                        break;
                    }

                    // ---------------- DESTINATION CONNECTION ----------------
                    using (SqlConnection destConn = new SqlConnection(ConfigReader.destinationConnStr))
                    {
                        destConn.Open();

                        using (SqlBulkCopy bulk = new SqlBulkCopy(destConn))
                        {
                            bulk.DestinationTableName = "StgEmployee";
                            bulk.BatchSize = 1000;
                            bulk.WriteToServer(sourceData);
                        }

                        DataTable resultTable = new DataTable();

                        using (SqlCommand processCmd = new SqlCommand("ProcessEmpBulk", destConn))
                        {
                            processCmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataAdapter da = new SqlDataAdapter(processCmd))
                            {
                                da.Fill(resultTable);
                            }
                        }

                        // ---------------- PREPARE TVP ----------------
                        DataTable tvp = new DataTable();
                        tvp.Columns.Add("emp_id", typeof(int));
                        tvp.Columns.Add("push_status", typeof(string));
                        tvp.Columns.Add("remark", typeof(string));

                        foreach (DataRow row in resultTable.Rows)
                        {
                            tvp.Rows.Add(
                                Convert.ToInt32(row["emp_id"]),
                                row["push_status"].ToString(),
                                row["remark"] == DBNull.Value ? null : row["remark"].ToString()
                            );
                        }

                        // ---------------- UPDATE SOURCE TABLE ----------------
                        using (SqlCommand updateCmd = new SqlCommand("UpdateEmpStatus", sourceConn))
                        {
                            updateCmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter p = updateCmd.Parameters.Add(
                                "@EmpStatus", SqlDbType.Structured);
                            p.Value = tvp;
                            p.TypeName = "EmpStatusType"; 

                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
