namespace Gymify.Persistence.SeedData.Models;

public record FriendInviteSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid SenderId { get; set; }
    public required Guid ReceiverId { get; set; }
    public required int Status { get; set; }
}
