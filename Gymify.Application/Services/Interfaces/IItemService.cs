using Gymify.Application.DTOs.Item;

namespace Gymify.Application.Services.Interfaces;

public interface IItemService
{
    Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId);
    Task<ItemDto> GetByIdAsync(Guid itemId);
    Task SetDefaultUserItemsAsync(Guid userProfileId);
}
