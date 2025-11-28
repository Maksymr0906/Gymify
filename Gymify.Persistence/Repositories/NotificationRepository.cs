using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class NotificationRepository(GymifyDbContext context)
    : Repository<Notification>(context), INotificationRepository
{
    private readonly GymifyDbContext _context = context;
    public async Task<int> GetUnreadCountAsync(Guid userProfileId)
    {
        return await _context.Notifications
            .CountAsync();
    }

    public async Task<List<Notification>> GetRecentAsync(Guid userProfileId, int count)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserProfileId == userProfileId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetAllUnreadByUserIdAsync(Guid userProfileId)
    {
        return await _context.Notifications
            .ToListAsync();
    }

    public async Task DeleteOlderThanAsync(DateTime date)
    {
        await _context.Notifications
            .Where(n => n.CreatedAt < date)
            .ExecuteDeleteAsync();
    }
}
