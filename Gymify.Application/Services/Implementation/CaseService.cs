using Gymify.Application.DTOs.Case;
using Gymify.Application.DTOs.Item;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CaseService(IUnitOfWork unitOfWork, INotificationService notificationService) : ICaseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationService _notificationService = notificationService;
    private readonly Random _random = new Random();

    public async Task<ICollection<CaseInfoDto>> GetAllUserCasesAsync(Guid userProfileId, bool ukranianVer)
    {
        var userCases = await _unitOfWork.CaseRepository.GetAllCasesByUserIdAsync(userProfileId);

        var casesDtos = userCases.Select(item => new CaseInfoDto
        {
            Id = item.Id,
            Name = ukranianVer ? item.NameUk : item.NameEn,
            Description = ukranianVer ? item.DescriptionUk : item.DescriptionEn,
            ImageUrl = item.ImageUrl,
            Type = (int)item.Type,
        }).ToList();

        return casesDtos;
    }

    public async Task<CaseInfoDto> GetCaseDetailsAsync(Guid caseId, bool ukranianVer)
    {
        var caseEntity = await _unitOfWork.CaseRepository.GetByIdAsync(caseId);

        return new CaseInfoDto()
        {
            Id = caseEntity.Id,
            Name = ukranianVer ? caseEntity.NameUk : caseEntity.NameEn,
            Description = ukranianVer ? caseEntity.DescriptionUk : caseEntity.DescriptionEn,
            ImageUrl = caseEntity.ImageUrl,
			Type = (int)caseEntity.Type
        };
    }



    public async Task GiveRewardByLevelUp(Guid userProfileId, int levelsUp)
    {
        if (levelsUp <= 0) return;

        var allCases = (await _unitOfWork.CaseRepository.GetAllAsync()).ToList();

        if (!allCases.Any())
        {
            throw new InvalidOperationException("No cases configured in the system to give as rewards.");
        }

        for (int i = 0; i < levelsUp; i++)
        {
            var randomCase = allCases[_random.Next(allCases.Count)];

            var userCase = new UserCase
            {
                Id = Guid.NewGuid(),
                UserProfileId = userProfileId,
                CaseId = randomCase.Id
            };

            /*await _notificationService.SendNotificationAsync(
                        userProfileId,
                        $"You received new case '{randomCase.Name}'.",
                        "/Inventory" // Клікати нікуди не треба, це просто інфо
                    );*/

            await _unitOfWork.UserCaseRepository.CreateAsync(userCase);
        }

        await _unitOfWork.SaveAsync();
    }

    public async Task GiveRewardByAchievement(Guid userProfileId, Guid rewardItemId)
    {
        var rewardItem = await _unitOfWork.ItemRepository.GetByIdAsync(rewardItemId);

        if (rewardItem == null)
        {
            throw new KeyNotFoundException($"Reward item with ID {rewardItemId} not found.");
        }

        var userItem = new UserItem
        {
            Id = Guid.NewGuid(),
            UserProfileId = userProfileId,
            ItemId = rewardItemId
        };

        /*await _notificationService.SendNotificationAsync(
                        userProfileId,
                        $"You received new case '{rewardItem.Name}'.",
                        "/Inventory" // Клікати нікуди не треба, це просто інфо
                    );*/

        await _unitOfWork.UserItemRepository.CreateAsync(userItem);
        await _unitOfWork.SaveAsync();
    }

    public async Task<OpenCaseResultDto> OpenCaseAsync(Guid userId, Guid caseId, bool ukranianVer)
	{
		var userCase = await _unitOfWork.UserCaseRepository
			.GetFirstByUserIdAndCaseIdAsync(userId, caseId);

		if (userCase == null)
			throw new Exception("No userCase found");

		var caseItems = await _unitOfWork.CaseItemRepository.GetAllByCaseIdAsync(caseId);

		if (caseItems.Count == 0)
			throw new Exception("Case has no rewards");

		var itemsIds = caseItems.Select(item => item.ItemId).ToList();
		var detailedItems = (await _unitOfWork.ItemRepository.GetByListOfIdAsync(itemsIds)).ToList();

		if (!detailedItems.Any())
			throw new Exception("Case items details not found");

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

		if (rewardsOfSameRarity.Count == 0)
			rewardsOfSameRarity = detailedItems.ToList(); // Fallback, якщо нема предметів такої рідкості

		int selectedIndex = _random.Next(rewardsOfSameRarity.Count);
		var selectedReward = rewardsOfSameRarity[selectedIndex]; // Це наш переможець!

		const int stripLength = 100; // Загальна довжина стрічки (як у прикладі)
		const int winningIndex = 78;  // Позиція, де завжди буде переможець

		var rouletteItems = new List<Item>();

		for (int i = 0; i < stripLength; i++)
		{
			if (i == winningIndex)
			{
				rouletteItems.Add(selectedReward); // Вставляємо переможця
			}
			else
			{
				// Вставляємо випадковий предмет з усіх можливих у цьому кейсі
				rouletteItems.Add(detailedItems[_random.Next(detailedItems.Count)]);
			}
		}

		var rouletteStripDto = rouletteItems.Select(i => new ItemDto
		{
			Id = i.Id,
			Name = ukranianVer ? i.NameUk : i.NameEn,
			Description = ukranianVer ? i.DescriptionUk : i.DescriptionEn,
			ImageURL = i.ImageURL,
			Rarity = (int)i.Rarity,
			Type = (int)i.Type
		});

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
			RouletteStrip = rouletteStripDto, // Повертаємо нову стрічку
                                              // SelectedIndex видалено

            // Інформація про переможця
            Name = ukranianVer ? selectedReward.NameUk : selectedReward.NameEn,
            Description = ukranianVer ? selectedReward.DescriptionUk : selectedReward.DescriptionEn,
            ImageURL = selectedReward.ImageURL,
			Rarity = (int)selectedReward.Rarity,
			Type = (int)selectedReward.Type
		};
	}
}
