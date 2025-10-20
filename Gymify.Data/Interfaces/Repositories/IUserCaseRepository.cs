using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserCaseRepository
{
    Task<UserCase> CreateAsync(UserCase entity);
    Task<ICollection<UserCase>> GetAllAsync();
    Task<ICollection<UserCase>> GetAllByUserIdAsync(Guid userId);
    Task<UserCase?> GetFirstByUserIdAndCaseIdAsync(Guid userId, Guid caseId);
    Task<UserCase> UpdateAsync(UserCase entity);
    Task<UserCase> DeleteFirstByUserIdAndCaseIdAsync(Guid userId, Guid caseId);
}
