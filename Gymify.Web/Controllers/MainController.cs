using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class MainController : Controller
    {
        public IActionResult Index()
        {
            return View("Main");
        }
    }
}
