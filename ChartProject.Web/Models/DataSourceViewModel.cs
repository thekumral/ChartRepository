namespace ChartProject.Web.Models
{
    public class DataSourcesViewModel
    {
        public List<string> Views { get; set; } = new List<string>();
        public List<string> Functions { get; set; } = new List<string>();
        public List<string> StoredProcedures { get; set; } = new List<string>();
    }
}
