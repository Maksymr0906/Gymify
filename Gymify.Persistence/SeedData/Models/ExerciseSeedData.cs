namespace Gymify.Persistence.SeedData.Models;

public record class ExerciseSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; init; }
    public required int Type { get; init; }
    public required string Description { get; init; }
    public required string VideoURL { get; init; }
    public required int BaseXP { get; init; }
    public required double DifficultyMultiplier { get; init; }
}
