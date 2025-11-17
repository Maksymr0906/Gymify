namespace Gymify.Application.DTOs.UserExercise;

public class AddUserExerciseToWorkoutRequestDto
{
    public Guid WorkoutId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Sets { get; set; } = 0;
    public int? Reps { get; set; } = 0;
    public int? Weight { get; set; } = 0;
    public TimeSpan? Duration { get; set; }
    public int ExerciseType { get; set; } = 1;
}
