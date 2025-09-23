namespace Gymify.Persistence.SeedData.Models;

public record CommentSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Content { get; init; }
    public required Guid AuthorId { get; init; }
}
