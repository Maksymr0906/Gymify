using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Data.Interfaces.Repositories;

public interface ICaseRepository : IRepository<Case>
{
    public Task<ICollection<Case>> GetAllCasesByUserIdAsync(Guid userProfileId);
}
