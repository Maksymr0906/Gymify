namespace Gymify.Application.Services.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(Guid targetUserId, string message, string link);
}
