using System;
using System.Data;

public static class MeterSchema
{
    public static DataTable Create()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Meter_Id", typeof(string));
        dt.Columns.Add("IntervalDateTime", typeof(DateTime));
        dt.Columns.Add("Avg_Voltage_V", typeof(decimal));
        dt.Columns.Add("BlkEngy_kWh", typeof(decimal));
        dt.Columns.Add("BlkEngy_kVAh", typeof(decimal));
        dt.Columns.Add("Avg_Current_A", typeof(decimal));
        return dt;
    }
}
