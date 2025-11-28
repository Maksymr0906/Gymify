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
        var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            await _unitOfWork.NotificationRepository.UpdateAsync(notification);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _unitOfWork.NotificationRepository.GetAllUnreadByUserIdAsync(userId);

        if (unreadNotifications.Any())
        {
            foreach (var n in unreadNotifications)
            {
                n.IsRead = true;
                // Update не обов'язково викликати для кожного, якщо контекст один
            }

            // Якщо метод UpdateAsync приймає список, краще використати його, 
            // або просто SaveAsync(), якщо об'єкти відстежуються EF Core
            await _unitOfWork.NotificationRepository.UpdateRangeAsync(unreadNotifications);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task SendNotificationAsync(Guid targetUserId, string message, string link)
    {
        // 1. Збереження в БД...
        var notification = new Notification
        {
            UserProfileId = targetUserId, 
            Content = message,     
            Link = link,          
            IsRead = false,       
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.NotificationRepository.CreateAsync(notification);
        await _unitOfWork.SaveAsync();

        // 2. Відправка (ми не знаємо, що там SignalR, нам байдуже)
        await _notifierService.PushAsync(targetUserId, "ReceiveNotification", new
        {
            message,
            link,
            id = notification.Id
        });
    }
}