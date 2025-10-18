namespace Gymify.Application.DTOs.UserExercise;

public class UserExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public int? Sets { get; set; }
    public int? Reps { get; set; }
    public int? Weight { get; set; }
    public TimeSpan? Duration { get; set; }
    public int EarnedXP { get; set; }
}
