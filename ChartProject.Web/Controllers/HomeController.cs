using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ChartProject.Web.Models;
using System.Text;

namespace ChartProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet("select-data-source")]
        public async Task<IActionResult> DataSourceSelection()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7213/api/Chart/all-data-sources");
                if (response.IsSuccessStatusCode)
                {
                    var dataSources = await response.Content.ReadFromJsonAsync<DataSourcesViewModel>();
                    return View(dataSources);
                }
                else
                {
                    _logger.LogError($"Veri kaynaklar� al�namad�. Durum kodu: {response.StatusCode}");
                    return View(new DataSourcesViewModel());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veri kaynaklar� al�n�rken bir hata olu�tu.");
                return StatusCode(500, $"�� hata: {ex.Message}");
            }
        }

        [HttpPost("chart-selection-page")]
        public async Task<IActionResult> ChartSelectionPage([FromForm] string dataSource, [FromForm] string functionParameter, [FromForm] string dataType)
        {
            if (string.IsNullOrWhiteSpace(dataSource))
            {
                return BadRequest("Se�ilen veri kayna�� bo� olamaz.");
            }

            TempData["SelectedDataSource"] = dataSource;
            TempData["FunctionParameter"] = functionParameter;
            TempData["DataType"] = dataType;

            var apiEndpoint = dataType switch
            {
                "Function" => "get-data-from-function",
                "StoredProcedure" => "get-data-from-stored-procedure",
                "View" => "get-data-from-view",
                _ => null
            };

            if (apiEndpoint == null)
            {
                return BadRequest("Bilinmeyen veri kayna�� t�r�.");
            }

            try
            {
                HttpResponseMessage response;
                string requestUri;

                switch (dataType)
                {
                    case "Function":
                        requestUri = $"https://localhost:7213/api/Chart/{apiEndpoint}?minValue={Uri.EscapeDataString(functionParameter)}";
                        var functionRequestBody = dataSource; // FunctionName do�rudan JSON g�vdesi
                        response = await _httpClient.PostAsync(requestUri, new StringContent($"\"{functionRequestBody}\"", Encoding.UTF8, "application/json"));
                        break;

                    case "StoredProcedure":
                        requestUri = $"https://localhost:7213/api/Chart/{apiEndpoint}";
                        var storedProcedureRequestBody = new
                        {
                            ProcedureName = dataSource
                        };
                        response = await _httpClient.PostAsJsonAsync(requestUri, storedProcedureRequestBody);
                        break;

                    case "View":
                        requestUri = $"https://localhost:7213/api/Chart/{apiEndpoint}";
                        var viewRequestBody = dataSource;
                        response = await _httpClient.PostAsJsonAsync(requestUri, viewRequestBody);
                        break;

                    default:
                        return BadRequest("Bilinmeyen veri kayna�� t�r�.");
                }

                if (response.IsSuccessStatusCode)
                {
                    var chartData = await response.Content.ReadFromJsonAsync<List<ChartDataDto>>();
                    ViewBag.ChartData = chartData;
                    return View("SelectChartType");
                }
                else
                {
                    _logger.LogError($"Veri al�namad�. Durum kodu: {response.StatusCode}");
                    return StatusCode(500, "Veri al�n�rken bir hata olu�tu.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grafik verileri al�n�rken bir hata olu�tu.");
                return StatusCode(500, $"�� hata: {ex.Message}");
            }
        }






        [HttpPost("chart-selection-confirm")]
        public async Task<IActionResult> ChartSelectionConfirm([FromForm] string chartType)
        {
            if (string.IsNullOrWhiteSpace(chartType))
            {
                return BadRequest("Grafik t�r� se�ilmelidir.");
            }

            var chartData = ViewBag.ChartData as List<ChartDataDto>;
            if (chartData == null || !chartData.Any())
            {
                return BadRequest("Grafik verileri mevcut de�il.");
            }

            ViewBag.ChartType = chartType;
            return PartialView("_ChartDisplayPartial");
        }
    }
}
