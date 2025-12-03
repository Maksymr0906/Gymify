using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Achievement : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string NameUk { get; set; } = string.Empty;
    public string DescriptionUk { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string TargetProperty { get; set; } = string.Empty;
    public double TargetValue { get; set; }
    public string ComparisonType { get; set; } = ">=";
    public Guid RewardItemId { get; set; } = Guid.Empty;

    public ICollection<UserAchievement> UserAchievements { get; set; } = [];
}
