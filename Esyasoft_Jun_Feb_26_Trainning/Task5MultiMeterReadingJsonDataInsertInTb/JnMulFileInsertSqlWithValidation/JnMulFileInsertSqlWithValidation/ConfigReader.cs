using System.Configuration;

namespace MeterBatchProcessor.Config
{
    public static class ConfigReader
    {
        public static string BasePath =>
            ConfigurationManager.AppSettings["BasePath"];

        public static int BatchSize =>
            int.Parse(ConfigurationManager.AppSettings["BatchSize"]);

        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
        public static string ProcessPath =
            System.IO.Path.Combine(BasePath, "process");
        public static string ErrorPath =
            System.IO.Path.Combine(BasePath, "error");
        public static string PendingPath =>
            System.IO.Path.Combine(BasePath, "pending");
    }
}
