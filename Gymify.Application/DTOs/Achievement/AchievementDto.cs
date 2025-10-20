namespace Gymify.Application.DTOs.Achievement;

public class AchievementDto
{
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required string IconUrl { get; set; } = string.Empty;
    public required Guid RewardItemId { get; set; } = Guid.Empty;
}
