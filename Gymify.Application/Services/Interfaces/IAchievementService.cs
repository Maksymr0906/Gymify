using Gymify.Data.Entities;

namespace Gymify.Application.Services.Interfaces;

public interface IAchievementService
{
    Task<List<Achievement>> CheckForAchievementsAsync(Guid userProfileId);
}
