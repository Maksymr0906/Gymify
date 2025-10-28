using Microsoft.AspNetCore.Mvc;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Gymify.Application.Services.Implementation;
using Gymify.Application.ViewModels.UserItems;

namespace Gymify.Web.Controllers
{
    [Authorize]
    public class UserItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ICaseService _caseService;

        public UserItemsController(IItemService itemService,ICaseService caseService)
        {
            _itemService = itemService;
            _caseService = caseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {   
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var items = await _itemService.GetAllUserItemsAsync(userId);
            var cases = await _caseService.GetAllUserCasesAsync(userId);

            var userItemsViewModel = new UserItemsViewModel()
            {
                Items = items,
                Cases = cases
            };

            return View("UserItems", userItemsViewModel);
        }

        [HttpPost]
        public IActionResult GoToCasePage(Guid caseId)
        {
            var caseEntity = _caseService.GetCaseDetailsAsync(caseId);
            if (caseEntity == null)
                return NotFound();

            return RedirectToAction("Details", "Case", new { id = caseId });
        }
    }
}
