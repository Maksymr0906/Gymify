namespace Gymify.Persistence.SeedData.Models;

public record FriendshipSeedData
{
    public required Guid UserId1 { get; init; }
    public required Guid UserId2 { get; init; }
}
