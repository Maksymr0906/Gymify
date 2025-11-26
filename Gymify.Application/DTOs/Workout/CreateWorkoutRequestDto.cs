namespace Gymify.Application.DTOs.Workout;

public class CreateWorkoutRequestDto
{
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Conclusion { get; set; } = string.Empty;
    public required bool IsPrivate { get; set; } = true;
    public required Guid UserProfileId { get; set; }
}
