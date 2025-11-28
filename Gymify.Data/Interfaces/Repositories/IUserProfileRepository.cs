using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile> GetByApplicationUserId(Guid applicationUserId);
    Task<UserProfile?> GetAllCredentialsAboutUserByIdAsync(Guid userProfileId);
    Task<(List<UserProfile> Users, int TotalCount)> GetLeaderboardPageAsync(int page, int pageSize);
    Task<int> GetUserRankByXpAsync(long userXp);
    Task<List<UserProfile>> SearchUsersAsync(string searchTerm, Guid currentUserId);
}
