using Gymify.Data.Entities;
using Gymify.Data.Enums;

namespace Gymify.Data.Interfaces.Repositories;

public interface ICaseRepository : IRepository<Case>
{
    Task<Case?> GetByCaseTypeAsync(CaseType type);
}
