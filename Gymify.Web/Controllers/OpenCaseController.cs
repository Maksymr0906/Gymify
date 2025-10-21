using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class OpenCaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
