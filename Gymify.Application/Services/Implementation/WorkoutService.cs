using Gymify.Application.DTOs.Achievement;
using Gymify.Application.DTOs.Workout;
using Gymify.Application.DTOs.WorkoutsCalendar;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Application.Services.Implementation;

public class WorkoutService(IUnitOfWork unitOfWork, IUserProfileService userProfileService, IAchievementService achievementService, ICaseService caseService)
    : IWorkoutService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserProfileService _userProfileService = userProfileService;
    private readonly IAchievementService _achievementService = achievementService;
    private readonly ICaseService _caseService = caseService;

    public async Task<CompleteWorkoutResponseDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model)
    {
        var workout = await _unitOfWork.WorkoutRepository.GetByIdWithDetailsAsync(model.WorkoutId);

        if (workout == null)
            throw new Exception("Workout not found");

        workout.Conclusion = model.Conclusions;

        var totalXp = workout.Exercises.Sum(e => e.EarnedXP);
        workout.TotalXP = totalXp;

        await _unitOfWork.WorkoutRepository.UpdateAsync(workout);

        var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(workout.UserProfileId);

        var oldLevel = userProfile.Level;
        await _userProfileService.AddXPAsync(workout.UserProfileId, totalXp);
        await _userProfileService.UpdateStatsAsync(workout.UserProfileId, workout.Id);

        bool isLevelUp = userProfile.Level > oldLevel;

        var newAchievements = await _achievementService.CheckForAchievementsAsync(userProfile.Id);

        // Вернути тіп кейса
        await _caseService.GenerateRewardsAsync(userProfile.Id, newAchievements, isLevelUp);

        await _unitOfWork.SaveAsync();

        return new CompleteWorkoutResponseDto
        {
            WorkoutDto = new WorkoutDto
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                Conclusion = workout.Conclusion,
                IsPrivate = workout.IsPrivate,
                TotalXP = workout.TotalXP,
                UserProfileId = workout.UserProfileId
            },
            AchievementDtos = newAchievements.Select(a => new AchievementDto
            {
                AchievementId = a.Id,
                Name = a.Name,
                Description = a.Description,
                IconUrl = a.IconUrl,
                RewardItemId = a.RewardItemId,
                TargetProperty = a.TargetProperty,
                TargetValue = a.TargetValue,
                ComparisonType = a.ComparisonType,
                Progress = 0,
                IsCompleted = false,
                UnlockedAt = a.CreatedAt
            }).ToList()
        };
    }

    public async Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model, Guid userProfileId)
    {
        var workout = new Workout
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            UserProfileId = userProfileId
        };

        await _unitOfWork.WorkoutRepository.CreateAsync(workout);
        await _unitOfWork.SaveAsync();

        return new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            Conclusion = workout.Conclusion,
            IsPrivate = workout.IsPrivate,
            TotalXP = workout.TotalXP,
            UserProfileId = workout.UserProfileId,
        };
    }

    public async Task<ICollection<WorkoutDto>> GetAllUserWorkoutsAsync(Guid userProfileId)
    {
        var workouts = await _unitOfWork.WorkoutRepository.GetAllByUserIdAsync(userProfileId);

        var workoutDtos = workouts.Select(workout => new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            Conclusion = workout.Conclusion,
            IsPrivate = workout.IsPrivate,
            TotalXP = workout.TotalXP,
            UserProfileId = workout.UserProfileId
        }).ToList();

        return workoutDtos;
    }

    public async Task<DateTime?> GetFirstWorkoutDate(Guid userId, bool onlyMy, string authorName)
    {
        return await _unitOfWork.WorkoutRepository.GetFirstWorkoutDateAsync(userId, onlyMy, authorName);
    }

    public async Task<List<WorkoutDayDto>> GetWorkoutsByDayPage(DateTime? anchorDate, Guid userId, string? authorName, int page, bool onlyMy, bool byDescending)
    {
        int pageSizeDays = 28;
        DateTime queryStartDate;
        DateTime queryEndDate;

        if (byDescending)
        {
            // Логіка для "новіші спочатку" (залишається як є)
            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(-(page * pageSizeDays));
            var startDate = endDate.AddDays(-pageSizeDays + 1);

            queryStartDate = startDate;
            queryEndDate = endDate.AddDays(1).AddTicks(-1);
        }
        else
        {
            // Логіка для "старіші спочатку" (використовує anchorDate)
            if (anchorDate == null)
            {
                // Якщо JS з якоїсь причини не надіслав дату, повертаємо порожній список
                return new List<WorkoutDayDto>();
            }

            var startDate = anchorDate.Value.AddDays(page * pageSizeDays);
            var endDate = startDate.AddDays(pageSizeDays - 1);

            queryStartDate = startDate;
            queryEndDate = endDate.AddDays(1).AddTicks(-1);
        }

        var workouts = await _unitOfWork.WorkoutRepository
            .GetUserWorkoutsFilteredAsync(userId, queryStartDate, queryEndDate, authorName, onlyMy,byDescending);

        var groupedWorkouts = workouts
            .GroupBy(w => w.CreatedAt.Date)
            .Select(group => new WorkoutDayDto
            {
                Date = group.Key,
                TotalXpForDay = onlyMy ? group.Sum(w => w.TotalXP) : 0,
                Workouts = group.Select(w => new WorkoutDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    CreatedAt = w.CreatedAt,
                    UserProfileId = w.UserProfileId,
                    AuthorName = w.UserProfile.ApplicationUser.UserName,
                    TotalXP = onlyMy ? w.TotalXP : 0
                }).ToList() // Тепер тренування всередині дня також будуть відсортовані
            })
            .ToList();

        return groupedWorkouts;
    }

}
