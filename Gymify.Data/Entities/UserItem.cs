namespace Gymify.Data.Entities;

public class UserItem
{
    public Guid UserId { get; set; }
    public Guid ItemId { get; set; }
    public User User { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
