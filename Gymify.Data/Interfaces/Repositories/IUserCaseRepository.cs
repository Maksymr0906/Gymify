using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserCaseRepository
{
    Task<UserCase> CreateAsync(UserCase entity);
    Task<ICollection<UserCase>> GetAllAsync();
    Task<ICollection<UserCase>> GetAllByUserIdAsync(Guid userId);
    Task<UserCase> UpdateAsync(UserCase entity);
    Task<UserCase> DeleteByIdAsync(Guid id);
}
