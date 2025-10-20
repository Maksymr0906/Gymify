using Gymify.Application.DTOs.Item;

namespace Gymify.Application.Services.Interfaces;

public interface IItemService
{
    Task<ICollection<ItemDto>> GetAllUserItemsAsync(Guid userProfileId);
}
