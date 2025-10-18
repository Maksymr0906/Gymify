using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserCaseRepository(GymifyDbContext context) : IUserCaseRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<UserCase> CreateAsync(UserCase entity)
    {
        await _context.UserCases.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<UserCase>> GetAllAsync()
    {
        return await _context.UserCases
            .Include(uc => uc.Case)
            .Include(uc => uc.UserProfile)
            .ToListAsync();
    }

    public async Task<UserCase> GetByIdAsync(Guid id)
    {
        var entity = await _context.UserCases
            .Include(uc => uc.Case)
            .Include(uc => uc.UserProfile)
            .FirstOrDefaultAsync(uc => uc.CaseId == id);
        if (entity == null)
            throw new Exception("UserCase not found");
        return entity;
    }

    public async Task<UserCase> UpdateAsync(UserCase entity)
    {
        _context.UserCases.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<UserCase> DeleteByIdAsync(Guid id)
    {
        var entity = await _context.UserCases.FirstOrDefaultAsync(uc => uc.CaseId == id);
        if (entity == null)
            throw new Exception("UserCase not found");

        _context.UserCases.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
