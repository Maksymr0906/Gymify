using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class UserExercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ExerciseType Type { get; set; }
    public int? Sets { get; set; } = 0;
    public int? Reps { get; set; } = 0;
    public int? Weight { get; set; } = 0;
    public TimeSpan? Duration { get; set; }
    public int EarnedXP { get; set; } = 0;
    public Guid WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
}
