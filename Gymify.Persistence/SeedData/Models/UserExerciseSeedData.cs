namespace Gymify.Persistence.SeedData.Models;

public record UserExerciseSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string NameEn { get; init; }
    public required string NameUk { get; init; }
    public required int Type { get; init; }
    public int? Sets { get; init; }
    public int? Reps { get; init; }
    public int? Weight { get; init; }
    public TimeSpan? Duration { get; init; }
    public required int EarnedXP { get; init; }
    public required Guid WorkoutId { get; init; }
}
