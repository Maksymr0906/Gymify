using Gymify.Application.DTOs.UserExercise;

public class AddUserExercisesBatchRequestDto
{
    public Guid WorkoutId { get; set; }
    public List<AddUserExerciseToWorkoutRequestDto> Exercises { get; set; } = new();
}
