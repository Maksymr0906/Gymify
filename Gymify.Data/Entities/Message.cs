namespace Gymify.Data.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public Guid SenderProfileId { get; set; }
    public Guid ReceiverProfileId { get; set; }
    public UserProfile SenderProfile { get; set; } = null!;
    public UserProfile ReceiverProfile { get; set; } = null!;
}
