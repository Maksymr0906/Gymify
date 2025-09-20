namespace Gymify.Data.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
}
