namespace Gymify.Data.Entities;

public class UserItem
{
    public Guid UserProfileId { get; set; }
    public Guid ItemId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
