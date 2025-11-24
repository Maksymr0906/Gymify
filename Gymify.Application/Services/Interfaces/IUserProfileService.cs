using Gymify.Application.ViewModels.Home;
using Gymify.Application.ViewModels.UserProfile;

namespace Gymify.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task<HomeViewModel> ReceiveUserLevelWorkouts(Guid userId);
    Task AddXPAsync(Guid userProfileId, int earnedXp);
    Task UpdateStatsAsync(Guid userProfileId, Guid workoutId);
    Task<UserProfileViewModel> GetUserProfileModel(Guid userProfileId);
    Task UpdateUserNameAsync(Guid userProfileId, string userName);
}
