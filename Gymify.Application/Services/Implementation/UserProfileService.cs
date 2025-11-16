using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.Home;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class UserProfileService(IUnitOfWork unitOfWork, ILevelingService levelingService) : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILevelingService _levelingService = levelingService;

    public async Task<HomeViewModel> ReceiveUserLevelWorkouts(Guid userId)
    {
        var user = await _unitOfWork.UserProfileRepository.GetByIdAsync(userId);
        var userWorkouts = await _unitOfWork.WorkoutRepository.GetLastWorkouts(userId);

        List<WorkoutDto> workoutsDtos = new();

        foreach(var workout in userWorkouts)
        {
            workoutsDtos.Add(new WorkoutDto
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                CreatedAt = workout.CreatedAt,
                TotalXP = workout.TotalXP
            });
        }

        int currentLevel = _levelingService.CalculateLevel(user.CurrentXP);

        double totalXpForCurrentLevel = _levelingService.GetTotalXpForLevel(currentLevel);
        double totalXpForNextLevel = _levelingService.GetTotalXpForLevel(currentLevel + 1);

        double xpNeededForThisLevel = totalXpForNextLevel - totalXpForCurrentLevel;
        double xpEarnedInThisLevel = user.CurrentXP - totalXpForCurrentLevel;

        double progressPercentage = (xpNeededForThisLevel > 0)
            ? (xpEarnedInThisLevel / xpNeededForThisLevel) * 100
            : 0;

        var viewModel = new HomeViewModel
        {
            Level = currentLevel, 
            XpEarnedInThisLevel = (int)xpEarnedInThisLevel,
            XpNeededForThisLevel = (int)xpNeededForThisLevel,
            ProgressPercentage = progressPercentage,
            LastWorkouts = workoutsDtos
        };

        return viewModel;
    }

    public async Task AddXPAsync(Guid userProfileId, int earnedXp)
    {
        var user = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);

        user.CurrentXP += earnedXp;

        int newLevel = _levelingService.CalculateLevel(user.CurrentXP);
        user.Level = newLevel;

        await _unitOfWork.UserProfileRepository.UpdateAsync(user);
        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateStatsAsync(Guid userProfileId, Guid workoutId)
    {
        var user = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);
        var workout = await _unitOfWork.WorkoutRepository.GetByIdWithDetailsAsync(workoutId);

        if (user == null || workout == null)
            throw new Exception("User or workout not found.");

        user.TotalWorkouts += 1;
        user.TotalWeightLifted += workout.Exercises.Sum(e => e.Weight ?? 0);

        user.TotalKmRunned += workout.Exercises
            .Where(e => e.Type == ExerciseType.Cardio)
            .Sum(e => (int)(e.Duration?.TotalMinutes ?? 0) / 10);

        int strengthCount = workout.Exercises.Count(e => e.Type == ExerciseType.Strength);
        int cardioCount = workout.Exercises.Count(e => e.Type == ExerciseType.Cardio);
        int flexibilityCount = workout.Exercises.Count(e => e.Type == ExerciseType.Flexibility);
        int balanceCount = workout.Exercises.Count(e => e.Type == ExerciseType.Balance);
        int enduranceCount = workout.Exercises.Count(e => e.Type == ExerciseType.Endurance);
        int mobilityCount = workout.Exercises.Count(e => e.Type == ExerciseType.Mobility);

        user.StrengthExercisesCompleted += strengthCount;
        user.CardioExercisesCompleted += cardioCount;
        user.FlexibilityExercisesCompleted += flexibilityCount;
        user.BalanceExercisesCompleted += balanceCount;
        user.EnduranceExercisesCompleted += enduranceCount;
        user.MobilityExercisesCompleted += mobilityCount;

        if (workout.CreatedAt.Date == DateTime.UtcNow.Date)
            user.WorkoutStreak += 1;
        else
            user.WorkoutStreak = 0;

        await _unitOfWork.UserProfileRepository.UpdateAsync(user);
        await _unitOfWork.SaveAsync();
    }
}
