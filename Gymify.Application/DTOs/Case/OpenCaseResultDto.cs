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
	// Замінили 'Rewards' на 'RouletteStrip'
	public IEnumerable<ItemDto> RouletteStrip { get; set; } = new List<ItemDto>();

	// SelectedIndex видалено

	// Ці поля залишаються - вони описують виграний предмет
	public string ItemName { get; set; } = string.Empty;
	public string ItemDescription { get; set; } = string.Empty;
	public ItemType ItemType { get; set; } = 0;
	public ItemRarity ItemRarity { get; set; } = ItemRarity.Common;
	public string ItemImageURL { get; set; } = string.Empty;
}
