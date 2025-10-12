namespace Gymify.Persistence.SeedData.Models;

public record UserEquipmentSeedData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid UserProfileId { get; init; }
    public required Guid AvatarId { get; init; }
    public required Guid BackgroundId { get; init; }
    public required Guid TitleId { get; init; }
    public required Guid FrameId { get; init; }
}
