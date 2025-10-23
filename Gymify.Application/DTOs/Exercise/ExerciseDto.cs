namespace Gymify.Application.DTOs.Exercise;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string VideoURL { get; set; } = string.Empty;
}
