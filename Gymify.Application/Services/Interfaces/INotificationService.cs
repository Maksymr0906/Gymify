using Gymify.Application.DTOs.Notification;

namespace Gymify.Application.Services.Interfaces;

public interface INotificationService
{
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task SendNotificationAsync(Guid targetUserId, string messageEn, string messageUk, string link);
    Task<List<NotificationDto>> GetNotificationsAsync(Guid currentUserProfileId, int amount, bool ukraineVer);
}
