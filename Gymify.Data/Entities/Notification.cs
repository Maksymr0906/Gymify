using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Notification : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = 0;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
