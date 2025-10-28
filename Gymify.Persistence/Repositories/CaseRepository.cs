using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class CaseRepository(GymifyDbContext context)
    : Repository<Case>(context), ICaseRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<ICollection<Case>> GetAllCasesByUserIdAsync(Guid userProfileId)
    {
        return await _context.UserCases
            .Include(ui => ui.Case)
            .Where(ui => ui.UserProfileId == userProfileId)
            .Select(ui => ui.Case)
            .ToListAsync();
    }
}
