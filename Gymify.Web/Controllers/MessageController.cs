using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
