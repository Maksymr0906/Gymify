using Gymify.Application.DTOs.Notification;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifierService _notifierService; 

    public NotificationService(IUnitOfWork unitOfWork, INotifierService notifierService)
    {
        _unitOfWork = unitOfWork;
        _notifierService = notifierService;
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        await _unitOfWork.NotificationRepository.DeleteByIdAsync(notificationId);
        await _unitOfWork.SaveAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var allNotifications = await _unitOfWork.NotificationRepository.GetAllUnreadByUserIdAsync(userId); // Або спеціальний метод GetAllByUserId

        if (allNotifications.Any())
        {
            await _unitOfWork.NotificationRepository.DeleteRangeAsync(allNotifications);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task SendNotificationAsync(Guid targetUserId, string messageEn, string messageUk, string link)
    {
        var notification = new Notification
        {
            UserProfileId = targetUserId, 
            ContentEn = messageEn,     
            ContentUk = messageUk,     
            Link = link
        };
        await _unitOfWork.NotificationRepository.CreateAsync(notification);
        await _unitOfWork.SaveAsync();

        await _notifierService.PushAsync(targetUserId, "ReceiveNotification", new
        {
            messageEn,
            messageUk,
            link,
            id = notification.Id
        });
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(Guid currentUserProfileId, int amount, bool ukraineVer)
    {
        var notifications = await _unitOfWork.NotificationRepository.GetRecentAsync(currentUserProfileId, amount);

        List<NotificationDto> notificationDtos = new();

        foreach (var notification in notifications)
        {
            notificationDtos.Add(new NotificationDto
            {
                Id = notification.Id,
                UserProfileId = notification.UserProfileId,
                Content = ukraineVer ? notification.ContentUk : notification.ContentEn,
                CreatedAt = notification.CreatedAt,
                Link = notification.Link,
                Type = notification.Type
            });
        }
        return notificationDtos;
    }
}