using ChartProject.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ChartProject.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost("Login/SetConnectionInfo")]
        public async Task<IActionResult> SetConnectionInfo([FromForm] ConnectionInfoDto connectionInfo)
        {
            if (connectionInfo == null)
            {
                ViewBag.Error = "Connection info cannot be null.";
                return View("LoginPage");
            }
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7213/api/Chart/set-connection-info", connectionInfo);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("DataSourceSelection", "Home");
                }
                else
                {
                    ViewBag.Error = "Bağlantı bilgileri ayarlanamadı.";
                    return View("LoginPage");
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (optional)
                ViewBag.Error = $"Bir hata oluştu: {ex.Message}";
                return View("LoginPage");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                ViewBag.Error = $"Bir hata oluştu: {ex.Message}";
                return View("LoginPage");
            }
        }
    }
}
