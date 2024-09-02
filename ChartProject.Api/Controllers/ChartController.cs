using ChartProject.Api.Models.Dtos;
using ChartProject.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChartProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        private readonly IChartService _chartService;
        private readonly ILogger<ChartController> _logger;

        public ChartController(IChartService chartService, ILogger<ChartController> logger)
        {
            _chartService = chartService;
            _logger = logger;
        }

        [HttpPost("set-connection-info")]
        public IActionResult SetConnectionInfo([FromBody] ConnectionInfoDto connectionInfo)
        {
            if (connectionInfo == null)
            {
                return BadRequest("Connection info cannot be null.");
            }
            try
            {
                GlobalConnectionInfo.ConnectionInfo = connectionInfo;
                _chartService.SetConnectionInfo(connectionInfo);
                return Ok("Connection info set successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while setting connection info.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost("get-data-from-view")]
        public async Task<IActionResult> GetDataFromView([FromBody] string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                return BadRequest("View name cannot be null or empty.");
            }

            try
            {
                var connectionInfo = GlobalConnectionInfo.ConnectionInfo;
                if (connectionInfo == null)
                {
                    return BadRequest("Connection info is not set.");
                }

                connectionInfo.DataSource = viewName;
                var chartData = await _chartService.GetChartDataAsync(connectionInfo);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving data from view.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("get-data-from-stored-procedure")]
        public async Task<IActionResult> GetDataFromStoredProcedure([FromBody] StoredProcedureRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.StoredProcedureName))
            {
                return BadRequest("Stored procedure name cannot be null or empty.");
            }

            try
            {
                var connectionInfo = GlobalConnectionInfo.ConnectionInfo;
                if (connectionInfo == null)
                {
                    return BadRequest("Connection info is not set.");
                }

                connectionInfo.DataSource = request.StoredProcedureName;

                var parameters = new Dictionary<string, object>
            {
                { "Category", request.Category } // Saklı prosedürde kullanacağımız parametre
            };

                var chartData = await _chartService.GetChartDataAsync(connectionInfo, parameters);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving data from stored procedure.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("get-data-from-function")]
        public async Task<IActionResult> GetDataFromFunction([FromBody] string functionName, [FromQuery] float minValue)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                return BadRequest("Function name cannot be null or empty.");
            }

            try
            {
                var connectionInfo = GlobalConnectionInfo.ConnectionInfo;
                if (connectionInfo == null)
                {
                    return BadRequest("Connection info is not set.");
                }

                connectionInfo.DataSource = functionName;

                var parameters = new Dictionary<string, object>
        {
            { "MinValue", minValue }
        };

                var chartData = await _chartService.GetChartDataAsync(connectionInfo, parameters);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving data from function.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost("add-chart-data")]
        public async Task<IActionResult> AddChartData([FromBody] ChartDataDTO chartData)
        {
            if (chartData == null)
            {
                return BadRequest("Chart data cannot be null.");
            }

            try
            {
                await _chartService.AddChartDataAsync(chartData);
                return Ok("Chart data added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding chart data.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("all-data-sources")]
        public async Task<IActionResult> GetAllDataSources()
        {
            var connectionInfo = GlobalConnectionInfo.ConnectionInfo;
            if (connectionInfo == null)
            {
                return BadRequest("Connection info is not set.");
            }
            try
            {
                var dataSources = await _chartService.GetAllDataSourcesAsync( connectionInfo);
                return Ok(dataSources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving data sources.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}
