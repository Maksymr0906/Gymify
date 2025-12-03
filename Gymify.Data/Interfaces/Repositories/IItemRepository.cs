using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Data.Interfaces.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<ICollection<Item>> GetAllItemsByUserIdAsync(Guid userProfileId, bool onlyOffical);
    Task<ICollection<Item>> GetItemsWithTypeByUserIdAsync(Guid userProfileId, ItemType itemType);
    Task<bool> IsOwnedByUserAsync(Guid itemId, Guid userProfileId);
    Task<ICollection<Item>> GetByListOfIdAsync(ICollection<Guid> itemsId);
}
