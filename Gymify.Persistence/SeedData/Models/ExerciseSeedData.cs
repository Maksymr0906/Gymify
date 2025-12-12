namespace Gymify.Persistence.SeedData.Models;

public record class ExerciseSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string NameEn { get; init; }
    public required string NameUk { get; init; }
    public required string DescriptionEn { get; init; }
    public required string DescriptionUk { get; init; }
    public required int Type { get; init; }
    public required string VideoURL { get; init; }
    public required int BaseXP { get; init; }
    public required double DifficultyMultiplier { get; init; }
    public required bool IsApproved { get; init; }
    public required bool IsRejected { get; set; }
    public string? RejectReason { get; set; }
}
