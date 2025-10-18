using Gymify.Data.Entities;

namespace Gymify.Application.Services.Interfaces;

public interface ICaseService
{
    Task<List<UserCase>> GenerateRewardsAsync(Guid userProfileId, List<Achievement> newAchievements, bool isLevelUp);
}
