using AutoMapper;
using Gymify.Application.DTOs.Achievement;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class AchievementService(IUnitOfWork unitOfWork, ICaseService caseService) : IAchievementService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICaseService _caseService = caseService;

    public async Task<List<Achievement>> UpdateUserAchievementsAsync(Guid userProfileId)
    {
        var achievements = await _unitOfWork.AchievementRepository.GetAllByUserId(userProfileId);
        var user = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);

        foreach (var achievement in achievements)
        {

            var userAchievement = achievement
                .UserAchievements
                .FirstOrDefault(ua => ua.UserProfileId == userProfileId);

            if (userAchievement is null)
                continue;

            var property = typeof(UserProfile).GetProperty(achievement.TargetProperty);
            if (property == null)

                if (property is null)
                continue;

            var userValue = property.GetValue(user);

            if (userValue is null)
                continue;

            double numericValue = Convert.ToDouble(userValue);

            userAchievement.Progress = numericValue;

            bool isCompleted = achievement.ComparisonType switch
            {
                ">=" => numericValue >= achievement.TargetValue,
                ">" => numericValue > achievement.TargetValue,
                "==" => Math.Abs(numericValue - achievement.TargetValue) < 0.0001,
                "<=" => numericValue <= achievement.TargetValue,
                "<" => numericValue < achievement.TargetValue,
                _ => false
            };

            if (isCompleted && !userAchievement.IsCompleted)
            {
                userAchievement.IsCompleted = true;
                userAchievement.UnlockedAt = DateTime.UtcNow;
                await _caseService.GiveRewardByAchievement(user.Id, achievement.RewardItemId);
            }
            else if (!isCompleted)
            {
                userAchievement.IsCompleted = false;
            }
        }

        await _unitOfWork.SaveAsync();

        return achievements.ToList();
    }

    public async Task<ICollection<AchievementDto>> GetAllAchievementsAsync()
    {
        var achievements = await _unitOfWork.AchievementRepository.GetAllAsync();

        return achievements.Select(a => new AchievementDto
        {
            AchievementId = a.Id,
            Name = a.Name,
            Description = a.Description,
            IconUrl = a.IconUrl,
            TargetProperty = a.TargetProperty,
            TargetValue = a.TargetValue,
            ComparisonType = a.ComparisonType,
            RewardItemId = a.RewardItemId,
            Progress = 0,
            IsCompleted = false,
            UnlockedAt = null
        }).ToList();
    }

    public async Task<ICollection<AchievementDto>> GetUserAchievementsAsync(Guid userProfileId)
    {
        var achievements = await _unitOfWork.AchievementRepository.GetAllByUserId(userProfileId);

        return achievements.Select(a =>
        {
            var userAchievement = a.UserAchievements.FirstOrDefault(ua => ua.UserProfileId == userProfileId);

            return new AchievementDto
            {
                AchievementId = a.Id,
                Name = a.Name,
                Description = a.Description,
                IconUrl = a.IconUrl,
                TargetProperty = a.TargetProperty,
                TargetValue = a.TargetValue,
                ComparisonType = a.ComparisonType,
                RewardItemId = a.RewardItemId,

                Progress = userAchievement?.Progress ?? 0,
                IsCompleted = userAchievement?.IsCompleted ?? false,
                UnlockedAt = userAchievement?.UnlockedAt
            };
        }).ToList();
    }

    public async Task SetupUserAchievementsAsync(Guid userProfileId)
    {
        var achievements = await _unitOfWork.AchievementRepository.GetAllAsync();

        var userAchievements = achievements.Select(a => new UserAchievement
        {
            UserProfileId = userProfileId,
            AchievementId = a.Id,
            Progress = 0,
            IsCompleted = false,
            UnlockedAt = DateTime.UtcNow,
        }).ToList();

        foreach (var ua in userAchievements)
        {
            await _unitOfWork.UserAchievementRepository.CreateAsync(ua);
        }

        await _unitOfWork.SaveAsync();
    }

    private bool IsAchievementCompleted(UserProfile user, Achievement achievement)
    {
        var property = typeof(UserProfile).GetProperty(achievement.TargetProperty);
        if (property == null)
            return false;

        var currentValue = Convert.ToDouble(property.GetValue(user) ?? 0);
        var targetValue = achievement.TargetValue;

        return achievement.ComparisonType switch
        {
            ">=" => currentValue >= targetValue,
            ">" => currentValue > targetValue,
            "==" => Math.Abs(currentValue - targetValue) < 0.0001,
            "<=" => currentValue <= targetValue,
            "<" => currentValue < targetValue,
            _ => false
        };
    }
}
