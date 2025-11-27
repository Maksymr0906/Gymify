using Gymify.Application.DTOs.Leaderboard;
using Gymify.Application.ViewModels.Leaderboard;

namespace Gymify.Application.Services.Interfaces;

public interface ILeaderboardService
{
    Task<LeaderboardViewModel> GetLeaderboardAsync(Guid currentUserId, int page = 1, int pageSize = 20);
}