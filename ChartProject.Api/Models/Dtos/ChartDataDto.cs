namespace ChartProject.Api.Models.Dtos
{
    public class ChartDataDTO
    {
        public string Label { get; set; }
        public float Value { get; set; }  // float, veritabanındaki 'float' türüne uygun
        public string Category { get; set; }

    }
}
