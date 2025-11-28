using Gymify.Application.DTOs.Leaderboard;

namespace Gymify.Application.ViewModels.Leaderboard;

public class LeaderboardViewModel
{
    public List<LeaderboardItemDto> TopUsers { get; set; } = new();
    public LeaderboardItemDto CurrentUser { get; set; } 

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
