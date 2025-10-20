using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class ItemRepository(GymifyDbContext context)
    : Repository<Item>(context), IItemRepository
{

}
