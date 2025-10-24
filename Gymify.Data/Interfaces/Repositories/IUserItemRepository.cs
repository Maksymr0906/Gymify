using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserItemRepository : IRepository<UserItem>
{
    Task<UserItem> DeleteByItemIdAsync(Guid id);
    Task<ICollection<UserItem>> GetAllByUserIdAsync(Guid userId);
}
