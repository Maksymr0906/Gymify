namespace Gymify.Persistence.SeedData.Models;

public record ItemSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int Type { get; init; }
    public required int Rarity { get; init; }
    public required string ImageURL { get; init; }
}
