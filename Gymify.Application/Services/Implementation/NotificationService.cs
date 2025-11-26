using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifierService _notifier; // <--- Використовуємо абстракцію

    public NotificationService(IUnitOfWork unitOfWork, INotifierService notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
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
        await _notifier.PushAsync(targetUserId, "ReceiveNotification", new
        {
            message,
            link,
            id = notification.Id
        });
    }
}