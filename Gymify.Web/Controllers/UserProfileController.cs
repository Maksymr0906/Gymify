using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
