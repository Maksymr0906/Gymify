using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class FriendInvite
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public InviteStatus Status { get; set; } = InviteStatus.Pending;
    public User Receiver { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
