namespace Gymify.Persistence.SeedData.Models;

public record UserCaseSeedData
{
    public required Guid UserId { get; init; }
    public required Guid CaseId { get; init; }
}
