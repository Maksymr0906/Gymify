using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class AchievementRepository(GymifyDbContext context)
    : Repository<Achievement>(context), IAchievementRepository
{
    private readonly GymifyDbContext _context = context;

    public async Task<ICollection<Achievement>> GetAllByUserId(Guid userId)
    {
        return await Entities
        .Include(a => a.UserAchievements)
        .Where(a => a.UserAchievements.Any(ua => ua.UserProfileId == userId))
        .ToListAsync();
    }

    public async Task<ICollection<Achievement>> GetAchievementsByUserIdAsync(Guid userId)
    {
        return await _context.UserAchievements
            .Where(ua => ua.UserProfileId == userId)
            .Include(ua => ua.Achievement)  
            .Select(ua => ua.Achievement)
            .ToListAsync();
    }
}
