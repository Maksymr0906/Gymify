namespace Gymify.Application.DTOs.UserExercise;

public class AddUserExerciseToWorkoutRequestDto
{
    public required Guid WorkoutId { get; set; }
    public required string Name { get; set; } = string.Empty;
    public int? Sets { get; set; } = 0;
    public int? Reps { get; set; } = 0;
    public int? Weight { get; set; } = 0;
    public TimeSpan? Duration { get; set; }
    public required int ExerciseType { get; set; } = 1;
}
