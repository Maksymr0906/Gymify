using Gymify.Application.DTOs.Achievement;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class AchievementService(IUnitOfWork unitOfWork, ICaseService caseService, INotificationService notificationService) : IAchievementService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICaseService _caseService = caseService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<List<Achievement>> UpdateUserAchievementsAsync(Guid userProfileId)
    {
        var achievements = await _unitOfWork.AchievementRepository.GetAllByUserId(userProfileId);
        var completedAchievements = new List<Achievement>();
        var user = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);

        foreach (var achievement in achievements)
        {
            var userAchievement = achievement
                .UserAchievements
                .FirstOrDefault(ua => ua.UserProfileId == userProfileId);
         
            if (userAchievement is null)
                continue;

            double? progress = GetUserPropertyValue(user, achievement.TargetProperty);
            if (progress == null)
                continue;

            userAchievement.Progress = progress.Value;

            bool isCompleted = achievement.ComparisonType switch
            {
                ">=" => progress.Value >= achievement.TargetValue,
                ">" => progress.Value > achievement.TargetValue,
                "==" => Math.Abs(progress.Value - achievement.TargetValue) < 0.0001,
                "<=" => progress.Value <= achievement.TargetValue,
                "<" => progress.Value < achievement.TargetValue,
                _ => false
            };

            if (isCompleted && !userAchievement.IsCompleted)
            {
                completedAchievements.Add(achievement);
                userAchievement.IsCompleted = true;
                userAchievement.UnlockedAt = DateTime.UtcNow;
                await _notificationService.SendNotificationAsync(
                        userProfileId,
                        $"You have completed '{achievement.Name}' achievement.",
                        "/Achievements" // Клікати нікуди не треба, це просто інфо
                    );
                await _caseService.GiveRewardByAchievement(user.Id, achievement.RewardItemId);
            }
            else if (!isCompleted)
            {
                userAchievement.IsCompleted = false;
            }
        }

        await _unitOfWork.SaveAsync();

        return completedAchievements;
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

    private double? GetUserPropertyValue(UserProfile user, string propertyName)
    {
        var property = typeof(UserProfile).GetProperty(propertyName);
        if (property == null)
            return null;

        var value = property.GetValue(user);
        if (value == null)
            return null;

        return Convert.ToDouble(value);
    }

}
