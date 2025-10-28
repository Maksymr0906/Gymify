namespace Gymify.Application.DTOs.Item;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageURL { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Rarity { get; set; }
    public string TypeName => ((Gymify.Data.Enums.ItemType)Type).ToString();
    public string RarityName => ((Gymify.Data.Enums.ItemRarity)Rarity).ToString();
}
