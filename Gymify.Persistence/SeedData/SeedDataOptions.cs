using Gymify.Persistence.SeedData.Models;

namespace Gymify.Persistence.SeedData;

public class SeedDataOptions
{
    public ApplicationUserSeedData[] ApplicationUsers { get; init; } = [];
    public AchievementSeedData[] Achievements { get; init; } = [];
    public CaseSeedData[] Cases { get; init; } = [];
    public CaseItemSeedData[] CaseItems { get; init; } = [];
    public ExerciseSeedData[] Exercises { get; init; } = [];
    public ImageSeedData[] Images { get; init; } = [];
    public ItemSeedData[] Items { get; init; } = [];
    public NotificationSeedData[] Notifications { get; init; } = [];
    public UserEquipmentSeedData[] UserEquipments { get; init; } = [];
    public UserExerciseSeedData[] UserExercises { get; init; } = [];
    public UserItemSeedData[] UserItems { get; init; } = [];
    public UserCaseSeedData[] UserCases { get; init; } = [];
    public UserProfileSeedData[] UserProfiles { get; init; } = [];
    public WorkoutSeedData[] Workouts { get; init; } = [];
}
