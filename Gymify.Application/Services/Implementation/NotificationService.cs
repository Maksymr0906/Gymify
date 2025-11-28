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
        // Просто видаляємо запис. Немає запису - немає сповіщення.
        await _unitOfWork.NotificationRepository.DeleteByIdAsync(notificationId);
        await _unitOfWork.SaveAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        // Отримуємо всі сповіщення юзера
        var allNotifications = await _unitOfWork.NotificationRepository.GetRecentAsync(userId, 100); // Або спеціальний метод GetAllByUserId

        if (allNotifications.Any())
        {
            // Видаляємо їх масово
            await _unitOfWork.NotificationRepository.DeleteRangeAsync(allNotifications);
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