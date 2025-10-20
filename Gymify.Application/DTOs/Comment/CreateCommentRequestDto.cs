using Gymify.Data.Enums;

namespace Gymify.Application.DTOs.Comment;

public class CreateCommentRequestDto
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid TargetId { get; set; }
    public CommentTargetType TargetType { get; set; }
}
