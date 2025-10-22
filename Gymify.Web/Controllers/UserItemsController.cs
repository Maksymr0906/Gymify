using Microsoft.AspNetCore.Mvc;
using Gymify.Application.Services.Interfaces;

namespace Gymify.Web.Controllers
{
    public class UserItemsController : Controller
    {
        private readonly IItemService _itemService;

        public UserItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var items = await _itemService.GetAllUserItemsAsync(userId);
            return View("UserItems", items);
        }
    }
}
