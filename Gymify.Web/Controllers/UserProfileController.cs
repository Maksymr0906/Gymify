using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserEquipmentService _userEquipmentService;
        public UserProfileController(IUserEquipmentService userEquipmentService)
        {
            _userEquipmentService = userEquipmentService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var equipment = await _userEquipmentService.GetUserEquipmentAsync(userId);



            return View("UserProfile", equipment);
        }
    }
}
