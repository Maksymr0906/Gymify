using Gymify.Data.Enums;

namespace Gymify.Application.DTOs.Comment;

public class CommentAdminDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public CommentTargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
}
