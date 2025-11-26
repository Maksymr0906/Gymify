using Gymify.Application.DTOs.Item;
using Gymify.Data.Enums;

namespace Gymify.Application.Services.Interfaces;

public interface IItemService
{
    Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId);
    Task<ICollection<ItemDto>> GetUserItemsWithTypeAsync(Guid userProfileId, ItemType itemType);
    Task<ItemDto> GetByIdAsync(Guid itemId);
    Task SetDefaultUserItemsAsync(Guid userProfileId);
}
