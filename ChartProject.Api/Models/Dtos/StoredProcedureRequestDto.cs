namespace ChartProject.Api.Models.Dtos
{
    public class StoredProcedureRequestDto
    {
        public string StoredProcedureName { get; set; }
        public int? Id { get; set; }      // Eğer Stored Procedure'da Id kullanılıyorsa
        public float? Value { get; set; } // Eğer Stored Procedure'da Value kullanılıyorsa
    }

}
