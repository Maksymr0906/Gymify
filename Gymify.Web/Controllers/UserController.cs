using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.UserItems;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    // Всі дії цього контролера можна викликати через чистий URL
    [Route("")]
    public class UserController : Controller
    {
        private readonly IUserEquipmentService _userEquipmentService;
        private readonly IItemService _itemService;
        private readonly ICaseService _caseService;

        public UserController(
            IUserEquipmentService userEquipmentService,
            IItemService itemService,
            ICaseService caseService)
        {
            _userEquipmentService = userEquipmentService;
            _itemService = itemService;
            _caseService = caseService;
        }

        [HttpGet("profile")]  // URL: /profile
        public async Task<IActionResult> Profile()
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var equipment = await _userEquipmentService.GetUserEquipmentAsync(userId);
            return View("Profile", equipment);
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

        [HttpPost] // URL: /goto-case
        public IActionResult GoToCasePage(Guid caseId)
        {
            var caseEntity = _caseService.GetCaseDetailsAsync(caseId);
            if (caseEntity == null)
                return NotFound();

            return RedirectToAction("Details", "Case", new { caseId = caseId });
        }

        [HttpGet("achievements")] // URL: /achievements
        public IActionResult Achievements()
        {
            return View("Achievements");
        }

        [HttpGet("workouts")] // URL: /workouts
        public IActionResult Workouts()
        {
            return View("Workouts");
        }

        [HttpGet("friends")] // URL: /friends
        public IActionResult Friends()
        {
            return View("Friends");
        }
    }
}
