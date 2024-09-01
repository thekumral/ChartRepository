using ChartProject.Api.Models.Dtos;

namespace ChartProject.Api.Services
{
    public interface IChartService
    {
        void SetConnectionInfo(ConnectionInfoDto connectionInfo);
        ConnectionInfoDto GetConnectionInfo();
        Task<IEnumerable<ChartDataDTO>> GetChartDataAsync(ConnectionInfoDto connectionInfo, Dictionary<string, object> parameters = null);
        Task AddChartDataAsync(ChartDataDTO request);
    }
}
