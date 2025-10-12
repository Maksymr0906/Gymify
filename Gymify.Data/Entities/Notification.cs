using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Notification : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = 0;
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
}
