namespace Gymify.Data.Entities;

public class UserEquipment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid AvatarId { get; set; }
    public Guid BackgroundId { get; set; }
    public Guid TitleId { get; set; }
    public Guid FrameId { get; set; }
    public Item Avatar { get; set; } = null!;
    public Item Background { get; set; } = null!;
    public Item Title { get; set; } = null!;
    public Item Frame { get; set; } = null!;
    public User User { get; set; } = null!;
}

