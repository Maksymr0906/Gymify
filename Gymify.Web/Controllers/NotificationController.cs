using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
