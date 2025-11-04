using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserCaseRepository(GymifyDbContext context) : Repository<UserCase>(context), IUserCaseRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<ICollection<UserCase>> GetAllByUserIdAsync(Guid userId)
    {
        var entities = await _context.UserCases
            .Include(uc => uc.Case)
            .Include(uc => uc.UserProfile)
            .Where(uc => uc.UserProfileId == userId)
            .ToListAsync();

        return entities;
    }
    public async Task<UserCase?> GetFirstByUserIdAndCaseIdAsync(Guid userId, Guid caseId)
    {
        var entity = await _context.UserCases
            .Include(uc => uc.Case)
            .Include(uc => uc.UserProfile)
            .FirstOrDefaultAsync(uc => uc.UserProfileId == userId && uc.CaseId == caseId);

        return entity;
    }

    public async Task<UserCase> DeleteFirstByUserIdAndCaseIdAsync(Guid userId, Guid caseId)
    {
        var entity = await _context.UserCases.FirstOrDefaultAsync(uc => uc.UserProfileId == userId && uc.CaseId == caseId);
        if (entity == null)
            throw new Exception("UserCase not found");

        _context.UserCases.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
