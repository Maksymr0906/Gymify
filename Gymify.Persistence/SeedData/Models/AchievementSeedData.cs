namespace Gymify.Persistence.SeedData.Models;

public record AchievementSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string NameEn { get; init; }
    public required string NameUk { get; init; }
    public required string DescriptionEn { get; init; }
    public required string DescriptionUk { get; init; }
    public required string IconUrl { get; init; }
    public required string TargetProperty { get; init; }
    public required double TargetValue { get; init; }
    public required string ComparisonType { get; init; }
    public required Guid RewardItemId { get; init; }
}
