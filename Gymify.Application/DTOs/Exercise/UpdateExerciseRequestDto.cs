using Gymify.Data.Enums;

namespace Gymify.Application.DTOs.Exercise;

public class UpdateExerciseRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BaseXP { get; set; } = 0;
    public double DifficultyMultiplier { get; set; }
    public ExerciseType Type { get; set; }
    public string VideoURL { get; set; } = string.Empty;
}
