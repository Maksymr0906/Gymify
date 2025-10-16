using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class UserEquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
