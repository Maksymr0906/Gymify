using Gymify.Application.DTOs.Case;
using Gymify.Application.DTOs.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.ViewModels.UserItems;

public class UserItemsViewModel
{
    public IEnumerable<CaseInfoDto> Cases { get; set; }
    public IEnumerable<ItemDto> Items { get; set; }
}
