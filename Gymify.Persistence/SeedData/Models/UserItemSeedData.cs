namespace Gymify.Persistence.SeedData.Models;

public record UserItemSeedData
{
    public required Guid UserProfileId { get; init; }
    public required Guid ItemId { get; init; }
}
