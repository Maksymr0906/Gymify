using Gymify.Application.DTOs.Case;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CaseService(IUnitOfWork unitOfWork) : ICaseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Random _random = new Random();

    public async Task GenerateRewardsAsync(Guid userProfileId, List<Achievement> newAchievements, bool isLevelUp)
    {
        var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);
        if (userProfile == null)
            throw new Exception("UserProfile not found");

        
        foreach (var achievement in newAchievements)
        {
            var itemToGive = await _unitOfWork.ItemRepository
                    .GetByIdAsync(achievement.RewardItemId);

            if (itemToGive != null)
            {
                var userItem = new UserItem
                {
                    UserProfileId = userProfile.Id,
                    ItemId = itemToGive.Id
                };

                await _unitOfWork.UserItemRepository.CreateAsync(userItem);
            }
        }

        if (isLevelUp)
        {
            var allCases = (await _unitOfWork.CaseRepository.GetAllAsync()).ToList();
            if (allCases.Any())
            {
                var randomCase = allCases[_random.Next(allCases.Count)];

                var userCase = new UserCase
                {
                    UserProfileId = userProfile.Id,
                    CaseId = randomCase.Id
                };

                await _unitOfWork.UserCaseRepository.CreateAsync(userCase);
            }
        }


        await _unitOfWork.SaveAsync();
    }

    public async Task<OpenCaseResultDto> OpenAsync(Guid userId, Guid caseId)
    {
        var userCase = await _unitOfWork.UserCaseRepository
            .GetFirstByUserIdAndCaseIdAsync(userId, caseId);

        if (userCase == null)
            throw new Exception("No cases available for this user");

        var caseEntity = await _unitOfWork.CaseRepository.GetByIdAsync(caseId);
        var caseItems = await _unitOfWork.CaseItemRepository.GetAllByCaseIdAsync(caseId);

        if (caseEntity == null || !caseItems.Any()) 
            throw new Exception("Case has no rewards");

        var itemsIds = new List<Guid>();

        foreach(var item in caseItems)
        {
            itemsIds.Add(item.ItemId);
        }

        var detailedItems = await _unitOfWork.ItemRepository.GetByListOfIdAsync(itemsIds);

        var roll = _random.Next(1, 33);
        ItemRarity targetRarity;

        if (roll <= 2)
            targetRarity = ItemRarity.Legendary;
        else if (roll <= 4)
            targetRarity = ItemRarity.Epic;
        else if (roll <= 8)
            targetRarity = ItemRarity.Rare;
        else if (roll <= 16)
            targetRarity = ItemRarity.Uncommon;
        else
            targetRarity = ItemRarity.Common;

        var rewardsOfSameRarity = detailedItems
            .Where(r => r.Rarity == targetRarity)
            .ToList();

        if (!rewardsOfSameRarity.Any())
            rewardsOfSameRarity = detailedItems.ToList();

        var selectedReward = rewardsOfSameRarity[_random.Next(rewardsOfSameRarity.Count)];

        var userReward = new UserItem
        {
            UserProfileId = userId,
            ItemId = selectedReward.Id,
        };

        await _unitOfWork.UserItemRepository.CreateAsync(userReward);

        await _unitOfWork.UserCaseRepository.DeleteFirstByUserIdAndCaseIdAsync(userId, caseId);

        await _unitOfWork.SaveAsync();

        return new OpenCaseResultDto()
        {
            Name = selectedReward.Name,
            Description = selectedReward.Description,
            ImageURL = selectedReward.ImageURL,
            Rarity = selectedReward.Rarity,
            Type = selectedReward.Type
        };
    }
}
