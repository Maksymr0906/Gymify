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
            existingExercise = new Exercise
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = string.Empty,
                CreatedAt = DateTime.Now,
                Type = (ExerciseType)model.ExerciseType,
                BaseXP = DefaultPendingExerciseXP,
                DifficultyMultiplier = 1.0,
                IsApproved = false
            };

            await _unitOfWork.ExerciseRepository.CreateAsync(existingExercise);
        }

        int calculatedXP = CalculateXp(model, existingExercise);

        var userExercise = new UserExercise
        {
            Id = Guid.NewGuid(),
            Name = existingExercise.Name,
            Type = existingExercise.Type,
            Sets = model.Sets,
            Reps = model.Reps,
            Weight = model.Weight,
            Duration = new TimeSpan(0, 0, model.Duration ?? 0, 0),
            WorkoutId = model.WorkoutId,
            ExerciseId = existingExercise.Id,
            EarnedXP = calculatedXP
        };

        await _unitOfWork.UserExerciseRepository.CreateAsync(userExercise);
        await _unitOfWork.SaveAsync();

        return new UserExerciseDto
        {
            Id = userExercise.Id,
            WorkoutId = userExercise.WorkoutId,
            Name = userExercise.Name,
            Type = (int)userExercise.Type,
            Sets = userExercise.Sets,
            Reps = userExercise.Reps,
            Weight = userExercise.Weight,
            Duration = userExercise.Duration,
            EarnedXP = userExercise.EarnedXP,
        };
    }


    public async Task AddExercisesBatchAsync(Guid workoutId, List<AddUserExerciseToWorkoutRequestDto> exercises, Guid currentUserId)
    {
        foreach (var dto in exercises)
        {
            dto.WorkoutId = workoutId;
            await AddUserExerciseToWorkoutAsync(dto, currentUserId);
        }
    }

    public async Task SyncWorkoutExercisesAsync(Guid workoutId, List<UserExerciseDto> dtos, Guid userId)
    {
        var currentExercises = await _unitOfWork.UserExerciseRepository
            .GetAllByWorkoutIdAsync(workoutId);

        var incomingIds = dtos.Select(d => d.Id).ToList();

        var toDelete = currentExercises
            .Where(e => !incomingIds.Contains(e.Id))
            .ToList();

        if (toDelete.Any())
        {
            await _unitOfWork.UserExerciseRepository.DeleteRangeAsync(toDelete);
        }

        foreach (var dto in dtos)
        {
            var existingEntity = currentExercises.FirstOrDefault(e => e.Id == dto.Id);

            if (existingEntity != null)
            {
                existingEntity.Sets = dto.Sets;
                existingEntity.Reps = dto.Reps;
                existingEntity.Weight = dto.Weight;
                existingEntity.Duration = dto.Duration ?? TimeSpan.Zero;

                if (existingEntity.Exercise != null)
                {
                    var calcModel = new AddUserExerciseToWorkoutRequestDto
                    {
                        Sets = dto.Sets,
                        Reps = dto.Reps,
                        Weight = dto.Weight,
                        Duration = (int)(existingEntity.Duration?.TotalMinutes ?? 0)
                    };
                    existingEntity.EarnedXP = CalculateXp(calcModel, existingEntity.Exercise);
                }

                await _unitOfWork.UserExerciseRepository.UpdateAsync(existingEntity);
            }
            else
            {
                var baseExercise = await _unitOfWork.ExerciseRepository.GetByNameAsync(dto.Name);

                if (baseExercise == null)
                {
                    baseExercise = new Exercise
                    {
                        Id = Guid.NewGuid(),
                        Name = dto.Name,
                        Description = string.Empty,
                        CreatedAt = DateTime.UtcNow,
                        Type = (ExerciseType)dto.Type,
                        BaseXP = 10,
                        DifficultyMultiplier = 1.0,
                        IsApproved = false
                    };
                    await _unitOfWork.ExerciseRepository.CreateAsync(baseExercise);
                }

                var calcModel = new AddUserExerciseToWorkoutRequestDto
                {
                    Sets = dto.Sets,
                    Reps = dto.Reps,
                    Weight = dto.Weight,
                    Duration = dto.Duration.HasValue ? (int)dto.Duration.Value.TotalMinutes : 0
                };

                var newEntity = new UserExercise
                {
                    Id = dto.Id,
                    WorkoutId = workoutId,
                    ExerciseId = baseExercise.Id,
                    Name = baseExercise.Name,
                    Type = baseExercise.Type,
                    Sets = dto.Sets,
                    Reps = dto.Reps,
                    Weight = dto.Weight,
                    Duration = dto.Duration ?? TimeSpan.Zero,
                    EarnedXP = CalculateXp(calcModel, baseExercise)
                };

                await _unitOfWork.UserExerciseRepository.CreateAsync(newEntity);
            }
        }

        await _unitOfWork.SaveAsync();
    }

    // переписати
    public static int CalculateXp(AddUserExerciseToWorkoutRequestDto exerciseModel, Exercise userExercise)
    {
        int sets = exerciseModel.Sets ?? 0;
        int reps = exerciseModel.Reps ?? 0;
        int weight = exerciseModel.Weight ?? 0;
        double minutes = exerciseModel.Duration ?? 0;

        double factor = 1.0;

        if (weight > 0 && minutes == 0)
        {
            factor += (double)weight / 50.0; // кожні 50 кг — подвоєння
        }
        else if (minutes > 0 && weight == 0)
        {
            factor += minutes / 10.0; // кожні 10 хв — подвоєння
        }

        double xp = userExercise.DifficultyMultiplier * sets * Math.Max(reps, 1) * factor;

        // мінімальне XP, щоб не було нуля
        return (int)Math.Max(xp, userExercise.BaseXP);
    }
}

