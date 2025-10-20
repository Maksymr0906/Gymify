using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class ItemRepository(GymifyDbContext context)
    : Repository<Item>(context), IItemRepository
{
    private readonly GymifyDbContext _context = context;
    public async Task<ICollection<Item>> GetByListOfIdAsync(ICollection<Guid> itemsId)
    {
        if (itemsId == null || itemsId.Count == 0)
            return new List<Item>();

        var items = await _context.Items
            .Where(i => itemsId.Contains(i.Id))
            .ToListAsync();

        return items;
    }
}
