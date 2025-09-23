namespace Gymify.Persistence.SeedData.Models;

public record UserEquipmentSeedData
{
    public required Guid UserId { get; init; }
    public required Guid AvatarId { get; init; }
    public required Guid BackgroundId { get; init; }
    public required Guid BorderId { get; init; }
    public required Guid FrameId { get; init; }
}
