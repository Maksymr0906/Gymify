using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserAchievementRepository
{
    Task<UserAchievement> CreateAsync(UserAchievement entity);
    Task<ICollection<UserAchievement>> GetAllAsync();
    Task<ICollection<UserAchievement>> GetAllByUserId(Guid userProfileId);
    Task<UserAchievement> UpdateAsync(UserAchievement entity);
    Task<UserAchievement> DeleteByIdAsync(Guid id);
}
