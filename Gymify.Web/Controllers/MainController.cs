using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Gymify.Data.Entities;

namespace Gymify.Web.Controllers
{
    public class MainController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MainController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return View("Home");
            }
            else
            {
                return View("Start");
            }
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View("Privacy");
        }
    }
}
