using Gymify.Application.DTOs.Achievement;
using Gymify.Data.Entities;

namespace Gymify.Application.Services.Interfaces;

public interface IAchievementService
{
    Task<List<Achievement>> UpdateUserAchievementsAsync(Guid userProfileId);
    Task<ICollection<AchievementDto>> GetUserAchievementsAsync(Guid userProfileId, bool ukranianVer);
    Task SetupUserAchievementsAsync(Guid userProfileId);
}
