using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Notification : BaseEntity
{
    public Guid UserProfileId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = 0;
    public UserProfile UserProfile { get; set; } = null!;
}
