using Gymify.Persistence.SeedData.Models;

namespace Gymify.Persistence.SeedData;

public class SeedDataOptions
{
    public AchievementSeedData[] Achievements { get; init; } = [];
    public CaseSeedData[] Cases { get; init; } = [];
    public CommentSeedData[] Comments { get; init; } = [];
    public ExerciseSeedData[] Exercises { get; init; } = [];
    public FriendInviteSeedData[] FriendInvites { get; init; } = [];
    public FriendshipSeedData[] Friendships { get; init; } = [];
    public ItemSeedData[] Items { get; init; } = [];
    public MessageSeedData[] Messages { get; init; } = [];
    public NotificationSeedData[] Notifications { get; init; } = [];
    public UserAchievementSeedData[] UserAchievements { get; init; } = [];
    public UserCaseSeedData[] UserCases { get; init; } = [];
    public UserEquipmentSeedData[] UserEquipments { get; init; } = [];
    public UserItemSeedData[] UserItems { get; init; } = [];
    public UserRoleSeedData[] UserRoles { get; init; } = [];
    public WorkoutSeedData[] Workouts { get; init; } = [];
}
