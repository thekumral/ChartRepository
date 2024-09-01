using AutoMapper;
using ChartProject.Api.Models.Dtos;
using ChartProject.Api.Models;
using ChartProject.Api.Repositories;

namespace ChartProject.Api.Services
{
    public class ChartService : IChartService
    {
        private readonly IChartRepository _chartRepository;
        private readonly IMapper _mapper;
        private ConnectionInfoDto _connectionInfo;

        public ChartService(IChartRepository chartRepository, IMapper mapper)
        {
            _chartRepository = chartRepository;
            _mapper = mapper;
        }

        public void SetConnectionInfo(ConnectionInfoDto connectionInfo)
        {
            _connectionInfo = connectionInfo;
            _chartRepository.SetConnectionInfo(connectionInfo);
        }

        public ConnectionInfoDto GetConnectionInfo()
        {
            return _connectionInfo;
        }

        public async Task<IEnumerable<ChartDataDTO>> GetChartDataAsync(ConnectionInfoDto connectionInfo, Dictionary<string, object> dataSourceParameters = null)
        {
            string connectionString = $"Server={connectionInfo.ServerName};Database={connectionInfo.DatabaseName};Integrated Security=True;TrustServerCertificate=True;";
            IEnumerable<ChartData> chartData = null;

            if (connectionInfo.DataSource.StartsWith("sp_"))
            {
                chartData = await _chartRepository.GetChartDataFromStoredProcedureAsync(connectionInfo.DataSource, connectionString, dataSourceParameters);
            }
            else if (connectionInfo.DataSource.Contains("GetChartDataAboveValue"))
            {
                chartData = await _chartRepository.GetChartDataFromFunctionAsync(connectionInfo.DataSource, connectionString, dataSourceParameters);
            }
            else
            {
                chartData = await _chartRepository.GetChartDataFromViewAsync(connectionInfo.DataSource, connectionString);
            }

            return _mapper.Map<IEnumerable<ChartDataDTO>>(chartData);
        }


        public async Task AddChartDataAsync(ChartDataDTO request)
        {
            var chartData = _mapper.Map<ChartData>(request);
            await _chartRepository.AddChartDataAsync(chartData);
        }

    }
}
