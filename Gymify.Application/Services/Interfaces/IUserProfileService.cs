using Gymify.Application.ViewModels.Home;

namespace Gymify.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task<HomeViewModel> ReceiveUserLevelWorkouts(Guid userId);
    Task AddXPAsync(Guid userProfileId, int earnedXp);
    Task UpdateStatsAsync(Guid userProfileId, Guid workoutId);
}
