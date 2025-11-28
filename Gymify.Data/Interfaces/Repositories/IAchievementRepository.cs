using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IAchievementRepository : IRepository<Achievement>
{
    Task<ICollection<Achievement>> GetAllByUserId(Guid userId);
    Task<ICollection<Achievement>> GetAchievementsByUserIdAsync(Guid userProfileId);
}
