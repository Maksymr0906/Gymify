namespace Gymify.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task AddXPAsync(Guid userProfileId, int earnedXp);
    Task UpdateStatsAsync(Guid userProfileId, Guid workoutId);
}
