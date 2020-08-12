using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class AppsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult First()
        {
            return View();
        }

        public IActionResult Second()
        {
            return View();
        }

        public IActionResult Third()
        {
            return View();
        }
    }
}