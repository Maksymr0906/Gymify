namespace Gymify.Data.Entities;

public class Achievement : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl {  get; set; } = string.Empty;
    public ICollection<UserAchievement> UserAchievements { get; set; } = [];
}
