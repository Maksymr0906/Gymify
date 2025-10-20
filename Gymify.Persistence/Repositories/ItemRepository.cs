using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class ItemRepository(GymifyDbContext context)
    : Repository<Item>(context), IItemRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<ICollection<Item>> GetAllItemsByUserIdAsync(Guid userProfileId)
    {
        return await _context.UserItems
            .Include(ui => ui.Item)
            .Where(ui => ui.UserProfileId == userProfileId)
            .Select(ui => ui.Item)
            .ToListAsync();
    }

    public async Task<bool> IsOwnedByUserAsync(Guid itemId, Guid userProfileId)
    {
        return await Entities.AnyAsync(i => i.Id == itemId &&
                                            i.UserItems.Any(ui => ui.UserProfileId == userProfileId));
    }
}
