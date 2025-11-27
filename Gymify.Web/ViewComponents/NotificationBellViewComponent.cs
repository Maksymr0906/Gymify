using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.NotificationBell;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gymify.Web.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationBellViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirst("UserProfileId")?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return Content(""); 

            var userId = Guid.Parse(userIdString);

            var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadCountAsync(userId);
            var recentNotifications = await _unitOfWork.NotificationRepository.GetRecentAsync(userId, 5);

            var model = new NotificationBellViewModel
            {
                UnreadCount = unreadCount,
                Notifications = recentNotifications
            };

            return View(model);
        }
    }
}