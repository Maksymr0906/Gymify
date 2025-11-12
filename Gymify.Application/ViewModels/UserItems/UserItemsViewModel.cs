using Gymify.Application.DTOs.Case;
using Gymify.Application.DTOs.Item;

namespace Gymify.Application.ViewModels.UserItems;

public class UserItemsViewModel
{
    public IEnumerable<CaseInfoDto> Cases { get; set; }
    public IEnumerable<ItemDto> Items { get; set; }
}
