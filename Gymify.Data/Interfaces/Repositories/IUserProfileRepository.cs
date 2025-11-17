using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile> GetByApplicationUserId(Guid applicationUserId);
    Task<UserProfile?> GetAllCredentialsAboutUserByIdAsync(Guid userProfileId);
}
