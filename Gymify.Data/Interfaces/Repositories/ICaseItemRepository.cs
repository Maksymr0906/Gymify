using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface ICaseItemRepository
{
    Task<CaseItem> CreateAsync(CaseItem entity);
    Task<ICollection<CaseItem>> GetAllAsync();
    Task<ICollection<CaseItem>> GetAllByCaseIdAsync(Guid caseId);
    Task<CaseItem> UpdateAsync(CaseItem entity);
    Task<CaseItem> DeleteByIdAsync(Guid id);
}
