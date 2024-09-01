namespace ChartProject.Api.Models.Dtos
{
    public class ChartDataRequestDto
    {
        public string DataSource { get; set; }  // Function, Stored Procedure, or View name
        public string Type { get; set; }  // Type of the data source: sp_procedure, function, view
    }
}
