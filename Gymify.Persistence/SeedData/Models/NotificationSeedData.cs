namespace Gymify.Persistence.SeedData.Models;

public record NotificationSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string ContentEn { get; init; }
    public required string ContentUk { get; init; }
    public required int Type { get; init; }
    public required Guid UserProfileId { get; init; }
}
