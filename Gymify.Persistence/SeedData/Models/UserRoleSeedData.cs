namespace Gymify.Persistence.SeedData.Models;

public record UserRoleSeedData
{
    public required Guid UserId { get; init; }
    public required int RoleId { get; init; }
}