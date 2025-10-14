namespace Gymify.Data.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorProfileId { get; set; }
    public UserProfile AuthorProfile { get; set; } = null!;
}
