using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
