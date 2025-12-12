namespace Gymify.Application.DTOs.UserExercise;

public class UserExerciseDto
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public int? Sets { get; set; }
    public int? Reps { get; set; }
    public double? Weight { get; set; }
    public int EarnedXP { get; set; }
    public TimeSpan? Duration { get; set; }
    public double DurationMinutes
    {
        get => Duration.HasValue ? (double)Duration.Value.TotalMinutes : 0;
        set => Duration = TimeSpan.FromMinutes(value);
    }

    public bool IsPendingApproval { get; set; } = true;
}
