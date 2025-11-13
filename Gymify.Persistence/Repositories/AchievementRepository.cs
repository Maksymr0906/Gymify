using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class AchievementRepository(GymifyDbContext context)
    : Repository<Achievement>(context), IAchievementRepository
{
    public async Task<ICollection<Achievement>> GetAllByUserId(Guid userId)
    {
        return await Entities
        .Include(a => a.UserAchievements)
        .Where(a => a.UserAchievements.Any(ua => ua.UserProfileId == userId))
        .ToListAsync();
    }
}
