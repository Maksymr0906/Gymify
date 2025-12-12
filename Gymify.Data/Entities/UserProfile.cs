namespace Gymify.Data.Entities;

public class UserProfile : BaseEntity
{
    public Guid ApplicationUserId { get; set; }
    public int Level { get; set; } = 0;
    public long CurrentXP { get; set; } = 0;
    public int TotalWorkouts { get; set; } = 0;
    public int WorkoutStreak { get; set; } = 0;
    public double TotalWeightLifted { get; set; } = 0;
    public int TotalKmRunned { get; set; } = 0;
    public int StrengthExercisesCompleted { get; set; } = 0;
    public int CardioExercisesCompleted { get; set; } = 0;
    public int FlexibilityExercisesCompleted { get; set; } = 0;
    public int BalanceExercisesCompleted { get; set; } = 0;
    public int EnduranceExercisesCompleted { get; set; } = 0;
    public int MobilityExercisesCompleted { get; set; } = 0;
    public UserEquipment Equipment { get; set; } = null!;
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
    public virtual ApplicationUser? ApplicationUser { get; set; }
}
