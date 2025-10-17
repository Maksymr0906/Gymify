namespace Gymify.Application.Services.Interfaces;

public interface ICaseService
{
    Task GenerateRewardsAsync(Guid userProfileId);
}
