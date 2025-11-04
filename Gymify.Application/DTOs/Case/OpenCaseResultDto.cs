using Gymify.Application.DTOs.Item;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Case;
public class OpenCaseResultDto
{
	public IEnumerable<ItemDto> RouletteStrip { get; set; } = new List<ItemDto>();
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string ImageURL { get; set; } = string.Empty;
	public int Type { get; set; } = 0;
	public int Rarity { get; set; } = 0;
    public string TypeName => ((Gymify.Data.Enums.ItemType)Type).ToString();
    public string RarityName => ((Gymify.Data.Enums.ItemRarity)Rarity).ToString();
}
