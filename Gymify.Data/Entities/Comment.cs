namespace Gymify.Data.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
}
