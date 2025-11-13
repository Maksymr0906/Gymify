namespace Gymify.Application.DTOs.Achievement;

public class AchievementDto
{
    public required Guid AchievementId { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required string IconUrl { get; set; } = string.Empty;
    public required string TargetProperty { get; set; } = string.Empty;
    public required double TargetValue { get; set; }
    public required string ComparisonType { get; set; } = ">=";
    public required Guid RewardItemId { get; set; } = Guid.Empty;

    public required double Progress { get; set; } = 0;
    public required bool IsCompleted { get; set; } = false;
    public required DateTime? UnlockedAt { get; set; }
}
