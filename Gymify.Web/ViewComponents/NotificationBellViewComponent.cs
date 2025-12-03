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
        private readonly INotificationService _notificationService;

        public NotificationBellViewComponent(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirst("UserProfileId")?.Value;
            bool ukraineVer = true;////////////////////////////////////////////////////////

            if (string.IsNullOrEmpty(userIdString))
                return Content(""); 

            var userId = Guid.Parse(userIdString);

            var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadCountAsync(userId);
            var recentNotifications = await _notificationService.GetNotificationsAsync(userId, 20, ukraineVer);

            var model = new NotificationBellViewModel
            {
                UnreadCount = unreadCount,
                Notifications = recentNotifications,
                UkranianVer = ukraineVer
            };

            return View(model);
        }
    }
}