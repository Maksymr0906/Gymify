using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class FriendInvite
{
    public Guid SenderProfileId { get; set; }
    public Guid ReceiverProfileId { get; set; }
    public UserProfile SenderProfile { get; set; } = null!;
    public UserProfile ReceiverProfile { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
