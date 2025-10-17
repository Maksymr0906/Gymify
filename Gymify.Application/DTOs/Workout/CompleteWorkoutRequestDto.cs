namespace Gymify.Application.DTOs.Workout;

public class CompleteWorkoutRequestDto
{
    public Guid WorkoutId { get; set; }
    public string Conclusions { get; set; } = string.Empty;
}
