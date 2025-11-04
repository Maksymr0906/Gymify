using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
