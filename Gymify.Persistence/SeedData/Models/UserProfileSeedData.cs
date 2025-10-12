namespace Gymify.Persistence.SeedData.Models;

public record UserProfileSeedData
{
    public required Guid Id { get; init; }
    public required Guid ApplicationUserId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required int Level { get; init; }
    public required long CurrentXP { get; init; }
}
