namespace Gymify.Application.Services.Interfaces;

public interface IAchievementService
{
    Task CheckForAchievementsAsync(Guid userProfileId);
}
