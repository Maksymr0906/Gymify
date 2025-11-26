using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<int> GetUnreadCountAsync(Guid userProfileId);
    Task<List<Notification>> GetRecentAsync(Guid userProfileId, int count);
    Task<List<Notification>> GetAllUnreadByUserIdAsync(Guid userId);
    Task UpdateRangeAsync(IEnumerable<Notification> notifications);
    Task DeleteOlderThanAsync(DateTime date);
}
