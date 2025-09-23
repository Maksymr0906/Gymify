namespace Gymify.Persistence.SeedData.Models;

public record UserItemSeedData
{
    public required Guid UserId { get; init; }
    public required Guid ItemId { get; init; }
}
