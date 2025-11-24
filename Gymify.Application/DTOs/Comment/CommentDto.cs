using Gymify.Data.Enums;

namespace Gymify.Application.DTOs.Comment;

public class CommentDto
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorAvatarUrl { get; set; }
    public Guid TargetId { get; set; }
    public CommentTargetType TargetType { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool CanDelete { get; set; } 
}
