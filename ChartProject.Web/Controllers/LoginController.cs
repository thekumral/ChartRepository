using Microsoft.AspNetCore.Mvc;

namespace ChartProject.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult LoginPage()
        {
            return View();
        }
    }
}
