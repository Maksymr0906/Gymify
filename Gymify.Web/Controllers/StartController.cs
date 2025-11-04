using Gymify.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gymify.Web.Controllers
{
    public class StartController : Controller
    {
        private readonly ILogger<StartController> _logger;

        public StartController(ILogger<StartController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
