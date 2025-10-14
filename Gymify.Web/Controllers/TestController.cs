using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class TestController : Controller
    {
        // Доступ тільки для авторизованих користувачів
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // Доступ тільки для ролі "User"
        [Authorize(Roles = "User")]
        public IActionResult UserPage()
        {
            return View();
        }

        // Доступ тільки для ролі "Admin"
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            return View();
        }
    }
}
