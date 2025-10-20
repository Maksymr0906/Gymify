using Gymify.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Data.Interfaces.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<ICollection<Item>> GetAllItemsByUserIdAsync(Guid userProfileId);
    Task<bool> IsOwnedByUserAsync(Guid itemId, Guid userProfileId);
}
