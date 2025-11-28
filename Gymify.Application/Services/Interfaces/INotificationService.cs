namespace Gymify.Application.Services.Interfaces;

public interface INotificationService
{
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task SendNotificationAsync(Guid targetUserId, string message, string link);
}
