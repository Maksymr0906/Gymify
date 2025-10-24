using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserCaseRepository : IRepository<UserCase>
{
    Task<ICollection<UserCase>> GetAllByUserIdAsync(Guid userId);
    Task<UserCase?> GetFirstByUserIdAndCaseIdAsync(Guid userId, Guid caseId);
    Task<UserCase> DeleteFirstByUserIdAndCaseIdAsync(Guid userId, Guid caseId);
}
