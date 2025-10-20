using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<ICollection<Item>> GetByListOfIdAsync(ICollection<Guid> itemsId);
}
