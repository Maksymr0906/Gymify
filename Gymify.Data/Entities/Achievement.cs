using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Achievement : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string TargetProperty { get; set; } = string.Empty;
    public double TargetValue { get; set; }
    public string ComparisonType { get; set; } = ">=";
    public CaseType? RewardCaseType { get; set; }

    public ICollection<UserAchievement> UserAchievements { get; set; } = [];
}
