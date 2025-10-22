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
    public ICollection<Data.Entities.Item> Rewards { get; set; } = new List<Data.Entities.Item>();
    public int SelectedIndex { get; set; } = 0;
    public string ItemName { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public ItemType ItemType { get; set; } = 0;
    public ItemRarity ItemRarity { get; set; } = ItemRarity.Common;
    public string ItemImageURL { get; set; } = string.Empty;
}
