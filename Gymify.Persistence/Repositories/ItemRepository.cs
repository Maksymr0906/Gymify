using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class ItemRepository(GymifyDbContext context)
    : Repository<Item>(context), IItemRepository
{
}
