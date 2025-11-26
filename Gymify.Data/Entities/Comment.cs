using Gymify.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gymify.Data.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid TargetId { get; set; }
    public CommentTargetType TargetType { get; set; }
    [ForeignKey("AuthorId")]
    public UserProfile? Author { get; set; }
}

