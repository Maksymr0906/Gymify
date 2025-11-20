using Gymify.Application.DTOs.Achievement;
using Gymify.Application.DTOs.Workout;
using Gymify.Application.DTOs.WorkoutsFeed;
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

        userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(workout.UserProfileId);
        var newLevel = userProfile.Level;

        var levelsUp = newLevel - oldLevel;

        await _userProfileService.UpdateStatsAsync(workout.UserProfileId, workout.Id);

        await _caseService.GiveRewardByLevelUp(userProfile.Id, levelsUp);
        
        var newAchievements = await _achievementService.UpdateUserAchievementsAsync(userProfile.Id);

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
            DateTime startPoint = DateTime.UtcNow.Date;

            queryEndDate = startPoint.AddDays(-(page * pageSizeDays));
            queryStartDate = queryEndDate.AddDays(-pageSizeDays + 1);

            queryEndDate = queryEndDate.AddDays(1).AddTicks(-1);
        }
        else
        {
            if (anchorDate == null)
            {
                return new List<WorkoutDayDto>();
            }

            var startDate = anchorDate.Value.AddDays(page * pageSizeDays);
            var endDate = startDate.AddDays(pageSizeDays - 1);

            queryStartDate = startDate;
            queryEndDate = endDate.AddDays(1).AddTicks(-1);
        }

        var workouts = await _unitOfWork.WorkoutRepository
            .GetUserWorkoutsFilteredAsync(userId, queryStartDate, queryEndDate, authorName, onlyMy, byDescending);

        var groupedWorkouts = workouts
            .GroupBy(w => w.CreatedAt.Date)
            .Select(group => new WorkoutDayDto
            {
                Date = group.Key,
                TotalXpForDay = onlyMy ? group.Sum(w => w.TotalXP) : 0,
                Workouts = group
                    .OrderByDescending(w => w.CreatedAt)
                    .Select(w => new WorkoutDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        CreatedAt = w.CreatedAt,
                        UserProfileId = w.UserProfileId,
                        AuthorName = w.UserProfile.ApplicationUser.UserName,
                        TotalXP = onlyMy ? w.TotalXP : 0
                    }).ToList()
            })
            .ToList();

        return groupedWorkouts;
    }

    // WorkoutService.cs

    public async Task<List<WorkoutDayDto>> GetWorkoutsPage(
        Guid userId,
        string? authorName,
        bool onlyMy,
        bool byDescending,
        int page)
    {
        // Фіксований розмір блоку, який ми намагаємося взяти
        int pageSize = 1;

        // Отримуємо базовий IQueryable запит з фільтрами та сортуванням.
        // Припускається, що GetWorkoutsQuery існує у WorkoutRepository.
        var baseQuery = _unitOfWork.WorkoutRepository.GetWorkoutsQuery(
            userId, authorName, onlyMy, byDescending);

        int skipCount = page * pageSize;

        // 1. Беремо перший блок (pageSize)
        var initialWorkouts = await baseQuery
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync();

        if (initialWorkouts.Count == 0)
        {
            return new List<WorkoutDayDto>();
        }

        // 2. Визначаємо дату стику (Дата останнього тренування у початковому блоці)
        DateTime lastDayInBlock = initialWorkouts.Last().CreatedAt.Date;

        // 3. Динамічне розширення Take: Перевіряємо, чи є наступний елемент
        var nextWorkoutCheck = await baseQuery
            .Skip(skipCount + pageSize) // Пропускаємо весь початковий блок
            .FirstOrDefaultAsync();

        // 4. Якщо наступний елемент існує і належить до ТОГО Ж ДНЯ,
        // ми повинні знайти усі інші тренування цього дня, які залишилися.
        if (nextWorkoutCheck != null && nextWorkoutCheck.CreatedAt.Date == lastDayInBlock)
        {
            // 5. Запитуємо всі тренування цього дня, починаючи з позиції після початкового Take.

            // Створюємо новий запит, який шукає тренування з датою стику, 
            // що йдуть за межами початкового блоку.
            var allRemainingWorkouts = await baseQuery
                 .Skip(skipCount + pageSize) // Пропускаємо вже завантажені
                 .Where(w => w.CreatedAt.Date == lastDayInBlock) // Фільтруємо за датою стику
                 .ToListAsync();

            // 6. Додаємо ці "додаткові" тренування до нашого початкового списку
            initialWorkouts.AddRange(allRemainingWorkouts);
        }

        // 7. Групування та повернення результату (Логіка групування не змінюється)
        var groupedWorkouts = initialWorkouts
            .GroupBy(w => w.CreatedAt.Date)
            .Select(group => new WorkoutDayDto
            {
                Date = group.Key,
                TotalXpForDay = onlyMy ? group.Sum(w => w.TotalXP) : 0,
                Workouts = group
                    .OrderByDescending(w => w.CreatedAt)
                    .Select(w => new WorkoutDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        CreatedAt = w.CreatedAt,
                        UserProfileId = w.UserProfileId,
                        AuthorName = w.UserProfile?.ApplicationUser?.UserName,
                        TotalXP = onlyMy ? w.TotalXP : 0
                    }).ToList()
            })
            .ToList();

        // Фінальне сортування днів для відображення
        if (byDescending)
        {
            groupedWorkouts = groupedWorkouts.OrderByDescending(d => d.Date).ToList();
        }
        else
        {
            groupedWorkouts = groupedWorkouts.OrderBy(d => d.Date).ToList();
        }

        return groupedWorkouts;
    }
}
