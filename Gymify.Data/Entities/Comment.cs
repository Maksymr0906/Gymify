using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid TargetId { get; set; }
    public CommentTargetType TargetType { get; set; }
    public UserProfile? UserProfile { get; set; }
    public Workout? Workout { get; set; }
    public UserProfile? Author { get; set; }
}

