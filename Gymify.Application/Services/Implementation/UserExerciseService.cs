using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class UserExerciseService(IUnitOfWork unitOfWork) : IUserExersiceService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private const int DefaultPendingExerciseXP = 10;

    public async Task<UserExerciseDto> AddUserExerciseToWorkoutAsync(AddUserExerciseToWorkoutRequestDto model, Guid currentUserId)
    {
        var existingExercise = await _unitOfWork.ExerciseRepository
            .GetByNameAsync(model.Name);

        if (existingExercise == null)
        {
            var pendingExercise = new PendingExercise
            {
                Name = model.Name,
                Type = (ExerciseType)model.ExerciseType,
                SubmittedByUserId = currentUserId,
                Description = null,
                VideoURL = null,
                IsApproved = false
            };

            await _unitOfWork.PendingExerciseRepository.CreateAsync(pendingExercise);

            existingExercise = new Exercise
            {
                Name = model.Name,
                Type = (ExerciseType)model.ExerciseType,
                BaseXP = DefaultPendingExerciseXP,
                DifficultyMultiplier = 1.0
            };
        }

        var userExercise = new UserExercise
        {
            Name = existingExercise.Name,
            Type = existingExercise.Type,
            Sets = model.Sets,
            Reps = model.Reps,
            Weight = model.Weight,
            Duration = model.Duration,
            WorkoutId = model.WorkoutId,
            ExerciseId = existingExercise.Id,
            EarnedXP = existingExercise.BaseXP
        };

        await _unitOfWork.UserExerciseRepository.CreateAsync(userExercise);
        await _unitOfWork.SaveAsync();

        return new UserExerciseDto
        {
            Id = userExercise.Id,
            Name = userExercise.Name,
            Type = (int)userExercise.Type,
            Sets = userExercise.Sets,
            Reps = userExercise.Reps,
            Weight = userExercise.Weight,
            Duration = userExercise.Duration,
            EarnedXP = userExercise.EarnedXP,
        };
    }
}
