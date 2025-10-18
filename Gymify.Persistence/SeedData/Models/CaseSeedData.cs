namespace Gymify.Persistence.SeedData.Models;

public record CaseSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string ImageUrl { get; init; }
    public required int CaseType { get; init; }
}
