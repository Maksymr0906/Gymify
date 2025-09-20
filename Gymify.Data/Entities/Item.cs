using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Item : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; } = 0;
    public ItemRarity Rarity { get; set; } = 0;
    public string ImageURL { get; set; } = string.Empty;
    public ICollection<UserItem> UserItems { get; set; } = [];
}
