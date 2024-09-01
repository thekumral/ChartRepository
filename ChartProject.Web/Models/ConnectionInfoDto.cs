namespace ChartProject.Web.Models
{
        public class ConnectionInfoDto
        {
            public string ServerName { get; set; }
            public string DatabaseName { get; set; }
            public string DataSource { get; set; }
        }
        public static class GlobalConnectionInfo
        {
            public static ConnectionInfoDto ConnectionInfo { get; set; }
        }

}
