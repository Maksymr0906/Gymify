namespace Gymify.Data.Entities;

public class UserAchievement
{
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public double Progress { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public DateTime UnlockedAt { get; set; }
    public User User { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
