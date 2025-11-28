namespace Gymify.Application.DTOs.UserEquipment;

public class UserEquipmentDto
{
    public Guid AvatarId { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public Guid BackgroundId { get; set; }
    public string BackgroundUrl { get; set; } = string.Empty;
    public Guid TitleId { get; set; }
    public string TitleText { get; set; } = string.Empty;
    public Guid FrameId { get; set; }
    public string FrameUrl { get; set; } = string.Empty;
}
