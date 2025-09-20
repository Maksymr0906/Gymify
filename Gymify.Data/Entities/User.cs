namespace Gymify.Data.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int Level { get; set; } = 0;
    public long CurrentXP { get; set; } = 0;
    public UserEquipment Equipment { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<UserAchievement> UserAchievements { get; set; } = [];
    public ICollection<FriendInvite> SentFriendInvites { get; set; } = [];
    public ICollection<FriendInvite> ReceivedFriendInvites { get; set; } = [];
    public ICollection<Friendship> Friendships1 { get; set; } = [];
    public ICollection<Friendship> Friendships2 { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
    public ICollection<Message> ReceivedMessages { get; set; } = [];
    public ICollection<UserItem> UserItems { get; set; } = [];
    public ICollection<UserCase> UserCases { get; set; } = [];
    public ICollection<Workout> Workouts { get; set; } = [];
}
