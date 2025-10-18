using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class AchievementService(IUnitOfWork unitOfWork) : IAchievementService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<List<Achievement>> CheckForAchievementsAsync(Guid userProfileId)
    {
        var user = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);
        var achievements = await _unitOfWork.AchievementRepository.GetAllAsync();

        var newAchievements = new List<Achievement>();

        foreach (var achievement in achievements)
        {
            bool alreadyUnlocked = user.UserAchievements.Any(ua => ua.AchievementId == achievement.Id);
            if (alreadyUnlocked) continue;

            if (IsAchievementCompleted(user, achievement))
            {
                user.UserAchievements.Add(new UserAchievement
                {
                    UserProfileId = user.Id,
                    AchievementId = achievement.Id,
                    UnlockedAt = DateTime.UtcNow
                });
                newAchievements.Add(achievement);
            }
        }

        await _unitOfWork.UserProfileRepository.UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        return newAchievements;
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
