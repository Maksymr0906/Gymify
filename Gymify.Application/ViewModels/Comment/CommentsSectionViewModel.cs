using Gymify.Application.DTOs.Comment;

public class CommentsSectionViewModel
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public List<CommentDto> Items { get; set; } = new();
    public string NewCommentContent { get; set; } = string.Empty;
}