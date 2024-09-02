using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ChartProject.Web.Models;

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
                    _logger.LogError($"Veri kaynaklarý alýnamadý. Durum kodu: {response.StatusCode}");
                    return View(new DataSourcesViewModel());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veri kaynaklarý alýnýrken bir hata oluþtu.");
                return StatusCode(500, $"Ýç hata: {ex.Message}");
            }
        }

        [HttpPost("chart-selection-page")]
        public async Task<IActionResult> ChartSelectionPage([FromForm] string dataSource, [FromForm] string functionParameter)
        {
            if (string.IsNullOrWhiteSpace(dataSource))
            {
                return BadRequest("Seçilen veri kaynaðý boþ olamaz.");
            }

            TempData["SelectedDataSource"] = dataSource;
            TempData["FunctionParameter"] = functionParameter;

            var apiEndpoint = dataSource.Contains("Function")
                ? "get-data-from-function"
                : dataSource.Contains("StoredProcedure")
                ? "get-data-from-stored-procedure"
                : "get-data-from-view";

            try
            {
                HttpResponseMessage response;

                if (dataSource.Contains("Function"))
                {
                    // Eðer bir Function seçilmiþse, hem FunctionName hem de MinValue gönderiyoruz.
                    var requestBody = new
                    {
                        FunctionName = dataSource,
                        MinValue = functionParameter
                    };

                    response = await _httpClient.PostAsJsonAsync($"https://localhost:7213/api/Chart/{apiEndpoint}", requestBody);
                }
                else if (dataSource.Contains("StoredProcedure"))
                {
                    // Eðer bir Stored Procedure seçilmiþse, dataSource'ý isim olarak gönderiyoruz.
                    var requestBody = new
                    {
                        ProcedureName = dataSource
                    };

                    response = await _httpClient.PostAsJsonAsync($"https://localhost:7213/api/Chart/{apiEndpoint}", requestBody);
                }
                else
                {
                    // Eðer bir View seçilmiþse, sadece dataSource ismini string olarak gönderiyoruz.
                    response = await _httpClient.PostAsJsonAsync($"https://localhost:7213/api/Chart/{apiEndpoint}", dataSource);
                }

                if (response.IsSuccessStatusCode)
                {
                    var chartData = await response.Content.ReadFromJsonAsync<List<ChartDataDto>>();
                    ViewBag.ChartData = chartData;
                    return View("SelectChartType");
                }
                else
                {
                    _logger.LogError($"Veri alýnamadý. Durum kodu: {response.StatusCode}");
                    return StatusCode(500, "Veri alýnýrken bir hata oluþtu.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Grafik verileri alýnýrken bir hata oluþtu.");
                return StatusCode(500, $"Ýç hata: {ex.Message}");
            }
        }


        [HttpPost("chart-selection-confirm")]
        public async Task<IActionResult> ChartSelectionConfirm([FromForm] string chartType)
        {
            if (string.IsNullOrWhiteSpace(chartType))
            {
                return BadRequest("Grafik türü seçilmelidir.");
            }

            var chartData = ViewBag.ChartData as List<ChartDataDto>;
            if (chartData == null || !chartData.Any())
            {
                return BadRequest("Grafik verileri mevcut deðil.");
            }

            ViewBag.ChartType = chartType;
            return PartialView("_ChartDisplayPartial");
        }
    }
}
