namespace Gymify.Persistence.SeedData.Models;

public record ImageSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string FileName { get; init; }
    public required string FileExtension { get; init; }
    public required string Title { get; init; }
    public required string Url { get; init; }
}