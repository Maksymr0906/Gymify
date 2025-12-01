using Gymify.Data.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid SenderId { get; set; }
    public Guid ChatId { get; set; }
    public UserProfile Sender { get; set; } = null!;
    public Chat Chat { get; set; } = null!;
    public ICollection<MessageReadStatus> ReadStatuses { get; set; } = new List<MessageReadStatus>();
}