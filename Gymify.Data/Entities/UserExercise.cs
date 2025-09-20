namespace Gymify.Data.Entities;

public class UserExercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; } = 0;
    public int Reps { get; set; } = 0;
    public int? Weight { get; set; } = 0;
    public int EarnedXP { get; set; } = 0;
    public Guid WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
}
