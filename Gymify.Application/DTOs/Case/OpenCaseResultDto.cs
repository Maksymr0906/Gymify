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
    public List<Data.Entities.Item> Rewards { get; set; } = new List<Data.Entities.Item>();
    public int SelectedIndex { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; } = 0;
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    public string ImageURL { get; set; } = string.Empty;
}
