using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.UserItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gymify.Web.Controllers
{
    [Route("")]
    public class UserController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IItemService _itemService;
        private readonly ICaseService _caseService;
        private readonly IAchievementService _achievementService;

        public UserController(
            IUserProfileService userProfileService,
            IItemService itemService,
            ICaseService caseService,
            IAchievementService achievementService)
        {
            _userProfileService = userProfileService;
            _itemService = itemService;
            _caseService = caseService;
            _achievementService = achievementService;
        }

        [HttpGet("profile")]  // URL: /profile
        public async Task<IActionResult> Profile(Guid userId)
        {
            var loggedUserId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var model = await _userProfileService.GetUserProfileModel(userId);
            model.Editable = loggedUserId == userId ? true : false; 

            return View("Profile", model);
        }

        [HttpGet("inventory")] // URL: /inventory
        public async Task<IActionResult> Inventory()
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var items = await _itemService.GetAllUserItemsAsync(userId);
            var cases = await _caseService.GetAllUserCasesAsync(userId);

            var userItemsViewModel = new UserItemsViewModel()
            {
                Items = items,
                Cases = cases
            };

            return View("Inventory", userItemsViewModel);
        }

        [HttpPost]
        public IActionResult GoToCasePage(Guid caseId)
        {
            var caseEntity = _caseService.GetCaseDetailsAsync(caseId);
            if (caseEntity == null)
                return NotFound();

            return RedirectToAction("Details", "Case", new { caseId = caseId });
        }

        [HttpGet("achievements")]
        public async Task<IActionResult> Achievements()
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var achievements = await _achievementService.GetUserAchievementsAsync(userId);
            return View("Achievements", achievements);
        }

        [HttpGet("workouts")]
        public IActionResult Workouts()
        {
            return View("Workouts");
        }

        [HttpGet("friends")]
        public IActionResult Friends()
        {
            return View("Friends");
        }
    }
}
