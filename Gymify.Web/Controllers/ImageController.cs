using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class ImageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
