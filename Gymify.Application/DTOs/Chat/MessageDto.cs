namespace Gymify.Application.DTOs.Chat;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderAvatarUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsMe { get; set; } // Головне поле для UI (праворуч чи ліворуч)
    public bool IsEdited { get; set; }
}