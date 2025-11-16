namespace Gymify.Persistence.SeedData.Models;

public record UserAchievementSeedData
{
    public required Guid UserProfileId { get; init; }
    public required Guid AchievementId { get; init; }
    public required double Progress { get; set; } = 0;
    public required bool IsCompleted { get; set; } = false;
    public required DateTime UnlockedAt { get; set; }
}
