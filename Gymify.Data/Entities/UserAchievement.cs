namespace Gymify.Data.Entities;

public class UserAchievement
{
    public Guid UserProfileId { get; set; }
    public Guid AchievementId { get; set; }
    public double Progress { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public DateTime UnlockedAt { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
