using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Gymify.Data.Entities;
using Gymify.Application.ViewModels.Home;
using Gymify.Application.Services.Interfaces;

namespace Gymify.Web.Controllers
{
    public class MainController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserProfileService _userProfileService;

        public MainController(SignInManager<ApplicationUser> signInManager, IUserProfileService userProfileService)
        {
            _signInManager = signInManager;
            _userProfileService = userProfileService;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());

                var viewModel = await _userProfileService.ReceiveUserLevelWorkouts(user);

                return View("Home", viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Home", "Admin");
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
