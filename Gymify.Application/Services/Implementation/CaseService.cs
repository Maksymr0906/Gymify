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
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
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
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    UserProfileId = userProfile.Id,
                    CaseId = randomCase.Id
                };

                await _unitOfWork.UserCaseRepository.CreateAsync(userCase);
            }
        }


        await _unitOfWork.SaveAsync();
    }
    public async Task<CaseInfoDto> GetCaseDetailsAsync(Guid caseId)
    {
        var caseEntity = await _unitOfWork.CaseRepository.GetByIdAsync(caseId);

        return new CaseInfoDto()
        {
            CaseId = caseEntity.Id,
            CaseName = caseEntity.Name,
            CaseDescription = caseEntity.Description,
            CaseImageUrl = caseEntity.ImageUrl
        };
    }

    public async Task<OpenCaseResultDto> OpenCaseAsync(Guid userId, Guid caseId)
    {
		var userCase = await _unitOfWork.UserCaseRepository
                .GetFirstByUserIdAndCaseIdAsync(userId, caseId);

        if (userCase == null)
            throw new Exception("No userCase found");

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

		int selectedIndex = _random.Next(rewardsOfSameRarity.Count);
		var selectedReward = rewardsOfSameRarity[selectedIndex];

		var userReward = new UserItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            UserProfileId = userId,
            ItemId = selectedReward.Id,
        };

        await _unitOfWork.UserItemRepository.CreateAsync(userReward);

        await _unitOfWork.UserCaseRepository.DeleteFirstByUserIdAndCaseIdAsync(userId, caseId);

        await _unitOfWork.SaveAsync();

        return new OpenCaseResultDto()
        {
            Rewards = detailedItems,
            SelectedIndex = selectedIndex,
			ItemName = selectedReward.Name,
            ItemDescription = selectedReward.Description,
            ItemImageURL = selectedReward.ImageURL,
            ItemRarity = selectedReward.Rarity,
            ItemType = selectedReward.Type
        };
    }
}
