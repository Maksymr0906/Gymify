using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserItemRepository
{
    Task<UserItem> CreateAsync(UserItem entity);
    Task<ICollection<UserItem>> GetAllAsync();
    Task<ICollection<UserItem>> GetAllByUserIdAsync(Guid userId);
    Task<UserItem> UpdateAsync(UserItem entity);
    Task<UserItem> DeleteByIdAsync(Guid id);
}
