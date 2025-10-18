namespace Gymify.Application.DTOs.Workout;

public class WorkoutDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public int TotalXP { get; set; } = 0;
    public Guid UserProfileId { get; set; }

}
