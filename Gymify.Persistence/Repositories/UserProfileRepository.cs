using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserProfileRepository(GymifyDbContext context)
    : Repository<UserProfile>(context), IUserProfileRepository
{
    public async Task<UserProfile> GetByApplicationUserId(Guid applicationUserId)
    {
        return await Entities.FirstOrDefaultAsync(x => x.ApplicationUserId == applicationUserId);
    }

    public async Task<UserProfile?> GetAllCredentialsAboutUserByIdAsync(Guid userProfileId)
    {
        return await Entities
            .Include(u => u.ApplicationUser)
            .Include(u => u.Equipment).ThenInclude(ue => ue.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userProfileId);
    }
    public async Task<(List<UserProfile> Users, int TotalCount)> GetLeaderboardPageAsync(int page, int pageSize)
    {
        var query = Entities
            .AsNoTracking()
            .Include(u => u.ApplicationUser)
            .Include(u => u.Equipment).ThenInclude(e => e.Avatar)
            .OrderByDescending(u => u.CurrentXP);

        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<int> GetUserRankByXpAsync(long userXp)
    {
        var countBetter = await Entities.CountAsync(u => u.CurrentXP > userXp);
        return countBetter + 1;
    }
}
