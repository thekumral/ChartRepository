using ChartProject.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChartProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult ChartSelectionPage(string dataSourceType)
        {
            ViewBag.DataSourceType = dataSourceType;
            return View();
        }

        public IActionResult RenderChartPage(string dataSourceType, string chartType)
        {
            ViewBag.ChartType = chartType;
            ViewBag.Labels = new[] { "Ocak", "Þubat", "Mart", "Nisan", "Mayýs" };
            ViewBag.Data = new[] { 65, 59, 80, 81, 56 };

            return View();
        }
    }
}
