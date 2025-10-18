namespace Gymify.Persistence.SeedData.Models;

public record UserProfileSeedData
{
    public required Guid Id { get; init; }
    public required Guid ApplicationUserId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int TotalWorkouts { get; init; } = 0;
    public required int WorkoutStreak { get; init; } = 0;
    public required int TotalWeightLifted { get; init; } = 0;
    public required int TotalKmRunned { get; init; } = 0;
    public required int StrengthExercisesCompleted { get; init; } = 0;
    public required int CardioExercisesCompleted { get; init; } = 0;
    public required int FlexibilityExercisesCompleted { get; init; } = 0;
    public required int BalanceExercisesCompleted { get; init; } = 0;
    public required int EnduranceExercisesCompleted { get; init; } = 0;
    public required int MobilityExercisesCompleted { get; init; } = 0;

    public required int Level { get; init; }
    public required long CurrentXP { get; init; }
}
