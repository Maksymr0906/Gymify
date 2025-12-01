using Gymify.Application.DTOs.Item;
using Gymify.Data.Enums;

namespace Gymify.Application.Services.Interfaces;

public interface IItemService
{
    Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId, bool ukranianVer);
    Task<ICollection<ItemDto>> GetUserItemsWithTypeAsync(Guid userProfileId, ItemType itemType, bool ukranianVer);
    Task<ItemDto> GetByIdAsync(Guid itemId, bool ukranianVer);
    Task SetDefaultUserItemsAsync(Guid userProfileId);
}
