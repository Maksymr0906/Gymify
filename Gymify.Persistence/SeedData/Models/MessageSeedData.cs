namespace Gymify.Persistence.SeedData.Models;

public record class MessageSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Content { get; init; }
    public required bool IsRead { get; init; }
    public required Guid SenderId { get; init; }
    public required Guid ReceiverId { get; init; }
}
