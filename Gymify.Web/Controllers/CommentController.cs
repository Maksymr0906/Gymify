using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
