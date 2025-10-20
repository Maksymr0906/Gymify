using Gymify.Application.DTOs.Item;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class UserProfileService(IUnitOfWork unitOfWork) : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task AddXPAsync(Guid userProfileId, int earnedXp)
    {
        var user = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);

        user.CurrentXP += earnedXp;

        int newLevel = (int)Math.Floor(Math.Sqrt(user.CurrentXP / 100.0));
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
