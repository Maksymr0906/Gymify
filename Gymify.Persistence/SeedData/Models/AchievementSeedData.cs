namespace Gymify.Persistence.SeedData.Models;

public record AchievementSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string IconUrl { get; init; }
    public required string TargetProperty { get; init; }
    public required double TargetValue { get; init; }
    public required string ComparisonType { get; init; }
    public required Guid RewardItemId { get; init; }
}
