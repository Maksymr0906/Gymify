namespace Gymify.Persistence.SeedData.Models;

public record UserAchievementSeedData
{
    public required Guid UserId { get; init; }
    public required Guid AchievementId { get; init; }
    public required double Progress { get; init; }
    public required bool IsCompleted { get; init; }
    public required DateTime UnlockedAt { get; init; }
}
