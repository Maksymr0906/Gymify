using Gymify.Application.DTOs.Case;
using Gymify.Application.DTOs.Item;
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
            Id = caseEntity.Id,
            Name = caseEntity.Name,
            Description = caseEntity.Description,
            ImageUrl = caseEntity.ImageUrl,
			Type = (int)caseEntity.Type
        };
    }

	public async Task<OpenCaseResultDto> OpenCaseAsync(Guid userId, Guid caseId)
	{
		var userCase = await _unitOfWork.UserCaseRepository
			.GetFirstByUserIdAndCaseIdAsync(userId, caseId);

		if (userCase == null)
			throw new Exception("No userCase found");

		var caseItems = await _unitOfWork.CaseItemRepository.GetAllByCaseIdAsync(caseId);

		if (!caseItems.Any())
			throw new Exception("Case has no rewards");

		var itemsIds = caseItems.Select(item => item.ItemId).ToList();
		var detailedItems = (await _unitOfWork.ItemRepository.GetByListOfIdAsync(itemsIds)).ToList();

		if (!detailedItems.Any())
			throw new Exception("Case items details not found");

		// --- ЛОГІКА ВИЗНАЧЕННЯ ПЕРЕМОЖЦЯ (залишається твоя) ---
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

		// --- НОВА ЛОГІКА: ГЕНЕРАЦІЯ СТРІЧКИ РУЛЕТКИ ---

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

		// Мапимо згенеровану стрічку в DTO
		var rouletteStripDto = rouletteItems.Select(i => new ItemDto
		{
			Id = i.Id,
			Name = i.Name,
			Description = i.Description,
			ImageURL = i.ImageURL,
			Rarity = (int)i.Rarity,
			Type = (int)i.Type
		});

		// --- ЗБЕРЕЖЕННЯ РЕЗУЛЬТАТУ (залишається твоє) ---
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

		// --- ПОВЕРНЕННЯ РЕЗУЛЬТАТУ ---
		return new OpenCaseResultDto()
		{
			RouletteStrip = rouletteStripDto, // Повертаємо нову стрічку
											  // SelectedIndex видалено

			// Інформація про переможця
			Name = selectedReward.Name,
			Description = selectedReward.Description,
			ImageURL = selectedReward.ImageURL,
			Rarity = selectedReward.Rarity,
			Type = selectedReward.Type
		};
	}
}
