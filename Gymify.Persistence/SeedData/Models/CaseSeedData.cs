namespace Gymify.Persistence.SeedData.Models;

public record CaseSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string NameEn { get; init; }
    public required string NameUk { get; init; }
    public required string DescriptionEn { get; init; }
    public required string DescriptionUk { get; init; }
    public required string ImageUrl { get; init; }
    public required int CaseType { get; init; }
}
