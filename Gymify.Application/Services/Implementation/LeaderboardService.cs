using Gymify.Application.DTOs.Leaderboard;
using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.Leaderboard;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Gymify.Application.Services.Implementation;

public class LeaderboardService : ILeaderboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public LeaderboardService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<LeaderboardViewModel> GetLeaderboardAsync(Guid currentUserId, int page = 1, int pageSize = 20)
    {
        var friendships = await _unitOfWork.FriendshipRepository.GetAllForUserAsync(currentUserId);

        var friendsIds = friendships.Select(f => f.UserProfileId1 == currentUserId ? f.UserProfileId2 : f.UserProfileId1).ToList();

        var (usersEntities, totalUsers) = await _unitOfWork.UserProfileRepository
            .GetLeaderboardPageAsync(page, pageSize);

        var nonAdminUsers = new List<UserProfile>();
        foreach (var u in usersEntities)
        {
            var appUser = await _userManager.FindByIdAsync(u.ApplicationUserId.ToString());
            if (appUser != null && !await _userManager.IsInRoleAsync(appUser, "Admin"))
            {
                nonAdminUsers.Add(u);
            }
        }

        usersEntities = nonAdminUsers;

        var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        if (page > totalPages && totalPages > 0) page = totalPages;
        if (page < 1) page = 1;

        var usersOnPage = usersEntities.Select(u => new LeaderboardItemDto
        {
            UserId = u.Id,
            UserName = u.ApplicationUser.UserName ?? "Unknown",
            AvatarUrl = u.Equipment?.Avatar?.ImageURL ?? "https://localhost:7102/Images/DefaultAvatar.png",
            Level = u.Level,
            TotalXP = u.CurrentXP,
            IsMe = u.Id == currentUserId,
            IsFriend = friendsIds.Contains(u.Id)
        }).ToList();

        int startRank = (page - 1) * pageSize + 1;
        for (int i = 0; i < usersOnPage.Count(); i++)
        {
            usersOnPage[i].Rank = startRank + i;
        }

        var currentUserDto = await GetCurrentUserRankAsync(currentUserId);

        return new LeaderboardViewModel
        {
            TopUsers = usersOnPage,
            CurrentUser = currentUserDto,
            CurrentPage = page,
            TotalPages = totalPages
        };
    }

    private async Task<LeaderboardItemDto> GetCurrentUserRankAsync(Guid userId)
    {
        var myProfile = await _unitOfWork.UserProfileRepository.GetAllCredentialsAboutUserByIdAsync(userId);

        if (myProfile == null) return null;

        var myRank = await _unitOfWork.UserProfileRepository.GetUserRankByXpAsync(myProfile.CurrentXP);

        return new LeaderboardItemDto
        {
            Rank = myRank,
            UserId = myProfile.Id,
            UserName = myProfile.ApplicationUser?.UserName ?? "Me",
            AvatarUrl = myProfile.Equipment?.Avatar?.ImageURL ?? "https://localhost:7102/Images/DefaultAvatar.png",
            Level = myProfile.Level,
            TotalXP = myProfile.CurrentXP,
            IsMe = true
        };
    }
}
