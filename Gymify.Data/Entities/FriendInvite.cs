using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class FriendInvite
{
    public Guid SenderProfileId { get; set; }
    public Guid ReceiverProfileId { get; set; }
    public InviteStatus Status { get; set; } = InviteStatus.Pending;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public UserProfile ReceiverProfile { get; set; } = null!;
    public UserProfile SenderProfile { get; set; } = null!;
}
