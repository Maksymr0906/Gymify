using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Item : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameUk { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionUk { get; set; } = string.Empty;
    public ItemType Type { get; set; } = 0;
    public ItemRarity Rarity { get; set; } = 0;
    public string ImageURL { get; set; } = string.Empty;
    public ICollection<UserItem> UserItems { get; set; } = [];
}
