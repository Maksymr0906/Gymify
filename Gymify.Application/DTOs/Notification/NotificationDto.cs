using Gymify.Data.Enums;

namespace Gymify.Application.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserProfileId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
}
