using Gymify.Application.DTOs.Achievement;
using Gymify.Data.Entities;

namespace Gymify.Application.Services.Interfaces;

public interface IAchievementService
{
    Task<List<Achievement>> UpdateUserAchievementsAsync(Guid userProfileId);
    Task<ICollection<AchievementDto>> GetAllAchievementsAsync();
    Task<ICollection<AchievementDto>> GetUserAchievementsAsync(Guid userProfileId);
    Task SetupUserAchievementsAsync(Guid userProfileId);
}
