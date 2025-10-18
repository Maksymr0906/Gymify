using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class CaseRepository(GymifyDbContext context)
    : Repository<Case>(context), ICaseRepository
{
    public async Task<Case?> GetByCaseTypeAsync(CaseType type)
    {
        return await Entities.FirstOrDefaultAsync(c => c.Type == type);
    }
}
