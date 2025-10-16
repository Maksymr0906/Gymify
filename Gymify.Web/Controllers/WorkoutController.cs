using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class WorkoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
