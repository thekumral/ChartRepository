using ChartProject.Api.Models.Dtos;
using ChartProject.Api.Models;

namespace ChartProject.Api.Repositories
{
    public interface IChartRepository
    {
        Task<IEnumerable<ChartData>> GetChartDataFromStoredProcedureAsync(string storedProcedureName, string connectionString, Dictionary<string, object> parameters = null);
        Task<IEnumerable<ChartData>> GetChartDataFromFunctionAsync(string functionName, string connectionString, Dictionary<string, object> parameters = null);
        Task<IEnumerable<ChartData>> GetChartDataFromViewAsync(string viewName, string connectionString);
        Task AddChartDataAsync(ChartData chartData);
        void SetConnectionInfo(ConnectionInfoDto connectionInfo);
    }
}
