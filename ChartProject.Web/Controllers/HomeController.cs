using ChartProject.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChartProject.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger,HttpClient httpClient)
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
                if(response.IsSuccessStatusCode)
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

        

        [HttpGet("select-chart-type")]
        public IActionResult SelectChartType()
        {
            return View();
        }

        [HttpPost("chart-selection")]
        public IActionResult ChartSelection([FromForm] string selectedDataSource, [FromForm] string chartType)
        {
            if (string.IsNullOrWhiteSpace(selectedDataSource) || string.IsNullOrWhiteSpace(chartType))
            {
                return BadRequest("Selected data source or chart type cannot be null or empty.");
            }

            // Store selected data source and chart type in TempData
            TempData["SelectedDataSource"] = selectedDataSource;
            TempData["ChartType"] = chartType;

            return RedirectToAction("DisplayChart");
        }

        [HttpGet("display-chart")]
        public IActionResult DisplayChart()
        {
            // Retrieve selected data source and chart type from TempData
            var selectedDataSource = TempData["SelectedDataSource"] as string;
            var chartType = TempData["ChartType"] as string;

            if (string.IsNullOrWhiteSpace(selectedDataSource) || string.IsNullOrWhiteSpace(chartType))
            {
                return BadRequest("Selected data source or chart type is not set.");
            }

            ViewBag.SelectedDataSource = selectedDataSource;
            ViewBag.ChartType = chartType;

            return View();
        }
    }
}
