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

    public async Task<List<WorkoutDayDto>> GetWorkoutsByDayPage(Guid userId, int page)
    {
        int pageSizeDays = 28;
        // Визначаємо діапазон дат.
        // ВИКОРИСТОВУЄМО UTC, оскільки BaseEntity.CreatedAt, 
        // скоріш за все, зберігається в UTC (це best practice).
        var today = DateTime.UtcNow.Date;

        // Вираховуємо діапазон
        var endDate = today.AddDays(-(page * pageSizeDays));
        var startDate = endDate.AddDays(-pageSizeDays + 1);

        // Встановлюємо час для коректного запиту
        // (від 00:00:00 першого дня до 23:59:59 останнього)
        var queryStartDate = startDate;
        var queryEndDate = endDate.AddDays(1).AddTicks(-1);

        // 1. Отримуємо всі тренування за діапазон
        var workouts = await _unitOfWork.WorkoutRepository.GetAllUserWorkoutsInDateRange(userId, queryStartDate, queryEndDate);

        // 2. Групуємо їх на стороні сервера (в пам'яті)
        //    (оскільки CreatedAt має час, групуємо по .Date)
        var groupedWorkouts = workouts
            .GroupBy(w => w.CreatedAt.Date) // Ключ - це дата
            .Select(group => new WorkoutDayDto
            {
                Date = group.Key, // Дата дня

                // Статистика для теплокарти
                TotalXpForDay = group.Sum(w => w.TotalXP),

                // Список тренувань
                Workouts = group.Select(w => new WorkoutDto
                {
                    Id = w.Id,
                    Name = w.Name
                }).ToList()
            })
            .OrderByDescending(d => d.Date) // Найновіші дні зверху
            .ToList();

        return groupedWorkouts;
    }
}
