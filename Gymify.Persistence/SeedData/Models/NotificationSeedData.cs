namespace Gymify.Persistence.SeedData.Models;

public record NotificationSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Content { get; init; }
    public required int Type { get; init; }
    public required Guid UserId { get; init; }
}
