using ChartProject.Api.Models.Dtos;
using ChartProject.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChartProject.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        private static ConnectionInfoDto _connectionInfo;
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
                // GlobalConnectionInfo'ya bağlantı bilgilerini atama
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
                if (_connectionInfo == null)
                {
                    return BadRequest("Connection info is not set.");
                }

                _connectionInfo.DataSource = viewName;
                var chartData = await _chartService.GetChartDataAsync(_connectionInfo);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data from view.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("get-data-from-stored-procedure")]
        public async Task<IActionResult> GetDataFromStoredProcedure([FromBody] StoredProcedureRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.StoredProcedureName))
            {
                return BadRequest("Stored procedure name cannot be null or empty.");
            }

            try
            {
                if (_connectionInfo == null)
                {
                    return BadRequest("Connection info is not set.");
                }

                _connectionInfo.DataSource = request.StoredProcedureName;

                var parameters = new Dictionary<string, object>();
                if (request.Id.HasValue)
                {
                    parameters.Add("@Id", request.Id.Value);
                }
                if (request.Value.HasValue)
                {
                    parameters.Add("@Value", request.Value.Value);
                }

                var chartData = await _chartService.GetChartDataAsync(_connectionInfo, parameters);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data from stored procedure.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }









        [HttpPost("get-data-from-function")]
        public async Task<IActionResult> GetDataFromFunction([FromBody] string functionName, [FromQuery] float? minValue = null)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                return BadRequest("Function name cannot be null or empty.");
            }

            try
            {
                if (_connectionInfo == null)
                {
                    return BadRequest("Connection info is not set.");
                }

                _connectionInfo.DataSource = functionName;

                var parameters = new Dictionary<string, object>();
                if (minValue.HasValue)
                {
                    parameters.Add("MinValue", minValue.Value);
                }

                var chartData = await _chartService.GetChartDataAsync(_connectionInfo, parameters);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data from function.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("add-data")]
        public async Task<IActionResult> AddDataAsync([FromBody] ChartDataDTO chartData)
        {
            if (chartData == null)
            {
                return BadRequest("Request cannot be null.");
            }

            try
            {
                await _chartService.AddChartDataAsync(chartData);
                return Ok("Data added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding data.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
