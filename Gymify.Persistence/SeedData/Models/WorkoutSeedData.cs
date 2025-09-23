namespace Gymify.Persistence.SeedData.Models;

public record WorkoutSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Conclusion { get; init; }
    public required bool IsPrivate { get; init; }
    public required Guid UserId { get; init; }
}
